package algorithm

import (
	"bytes"
	"encoding/json"
	"fmt"
	"net/http"
	common "so-swirly/common/algorithm"
	"so-swirly/edgeservice/config"
	"time"
)

var fogServices map[string]common.FogNode
var tempMigrateNodes map[string]common.FogNode

func InitMesher() {
	fogServices = make(map[string]common.FogNode)
	tempMigrateNodes = make(map[string]common.FogNode)
	common.NodesUpdatedCallback = ProcessUpdatedPings
}

func GetFogServices() map[string]common.FogNode {
	return fogServices
}

func DeploySupportServicesFor(serviceName string) {
	fmt.Printf("Deploy support services for %s\n", serviceName)
	supportServices := config.Cfg.SupportServices[serviceName]

	for _, svc := range supportServices {
		bestNode := findBestServiceProvider(svc, nil)
		if bestNode != nil {
			fogServices[svc] = *bestNode
			updateServerFor(svc, bestNode.IP)
		}
	}
}

func findBestServiceProvider(svc string, except *common.FogNode) *common.FogNode {
	fmt.Printf("Finding service provider for %s\n", svc)
	nodes := common.GetKnownNodes()

	found := false

	_, known := fogServices[svc]
	//don't need to find one if it's already prepared for another deployment (unless we need another one, "except")
	if !known || except != nil {
		idx := 0
		//first, try to find a node which already has the service
		for found == false && idx < len(nodes) {
			fnode := nodes[idx]
			fmt.Printf("Trying fog node %s distance %f\n", fnode.Name, fnode.Distance)
			if (except == nil || nodes[idx].IP != except.IP) && nodes[idx].Distance < config.Cfg.MaxPing && hasServiceAvailable(fnode.IP, fnode.Name, svc) {
				fmt.Printf("Node conditions ok for node %s service %s, try to add\n", fnode.Name, svc)
				found := tryAddToNode(fnode.IP, fnode.Name, svc)
				if found {
					fmt.Printf("Added to %s for service %s\n", fnode.Name, svc)
					return &nodes[idx]
				}
			}
			idx++
		}

		fmt.Printf("No existing service found, retrying with closest\n")
		if except == nil {
			idx = 0
			//if not, try to add to the closest ones
			for found == false && idx < len(nodes) {
				if except == nil || nodes[idx].IP != except.IP {
					fnode := nodes[idx]
					fmt.Printf("Trying fog node %s\n", fnode.Name)
					found := tryAddToNode(fnode.IP, fnode.Name, svc)
					if found {
						fmt.Printf("Added to %s for service %s\n", fnode.Name, svc)
						return &nodes[idx]
					}
					idx++
				}
			}
		}
	}

	return nil
}

func RemoveSupportServicesFor(serviceName string) {
	supportServices := config.Cfg.SupportServices[serviceName]

	//do a check to see if these aren't required by other edge services
	for edgeSvc, supportSvcs := range config.Cfg.SupportServices {
		if edgeSvc != serviceName {
			supportServices = exclude(supportServices, supportSvcs)
		}
	}

	go func() {
		for _, svc := range supportServices {
			removeFromSupportService(svc)
		}
	}()
}

func exclude(list []string, exclude []string) []string {
	newList := []string{}

	for _, svc := range list {
		include := true
		for _, excl := range exclude {
			if svc == excl {
				include = false
			}
		}
		if include {
			newList = append(newList, svc)
		}
	}

	return newList
}

func removeFromSupportService(svc string) {
	fnode := fogServices[svc]
	success := tryRemoveFromNode(fnode.IP, fnode.Name, svc)

	tries := 0
	for !success && tries < 10 {
		time.Sleep(5 * time.Second)
		success = tryRemoveFromNode(fnode.IP, fnode.Name, svc)
		tries++
	}
	delete(fogServices, svc)
}

//don't forget periodic ping updates that can influence deployed services... implement somehow
func ProcessUpdatedPings() {
	go func() {
		nodes := common.GetKnownNodes()

		for svc, fnode := range fogServices {
			dist := getNodePing(nodes, fnode)
			//only do something about it if we crossed the maxping threshold
			if dist == -1 || dist > config.Cfg.MaxPing*2 || (dist > config.Cfg.MaxPing && fnode.Distance < config.Cfg.MaxPing) {
				//this means we can probably get a better node, and should try
				removeFromSupportService(svc)
				newnode := findBestServiceProvider(svc, nil)
				fogServices[svc] = *newnode
				updateServerFor(svc, newnode.IP)
			}
		}
	}()
}

func getNodePing(nodes []common.FogNode, node common.FogNode) float32 {
	i := 0
	for i < len(nodes) {
		if nodes[i].IP == node.IP {
			return nodes[i].Distance
		}
	}
	return -1
}

func TryMigrateService(fogService string) {
	bestNode := findBestServiceProvider(fogService, nil)
	fnode := fogServices[fogService]
	if bestNode != nil {
		tempMigrateNodes[fogService] = *bestNode
		//notify current fog node to migrate

		notifyMigrateSuccess(fnode.IP, fnode.Name, fogService)
	} else {
		//notify current fog node to cancel
		notifyMigrateFailed(fnode.IP, fnode.Name, fogService)
	}
}

func MigrateService(fogService string) {
	//no need to remove from current node, it does that while notifying all its clients
	fogServices[fogService] = tempMigrateNodes[fogService]
	updateServerFor(fogService, fogServices[fogService].IP)
}

func CancelMigrate(fogService string) {
	fnode := tempMigrateNodes[fogService]
	tryRemoveFromNode(fnode.IP, fnode.Name, fogService)
	delete(tempMigrateNodes, fogService)
}

func updateServerFor(service string, ip string) {
	Locator.UpdateServiceLocation(service, ip)
}

func hasServiceAvailable(ip string, node string, svc string) bool {
	port := getPort(node)
	ip = getIP(ip)

	fullURL := fmt.Sprintf("http://%s:%d/%s", ip, port, config.Cfg.FogServiceRunningURL) //"isServiceRunning")
	fmt.Printf("Checking %s for service %s\n", fullURL, svc)
	jsonBytes, _ := json.Marshal(svc)
	response, err := http.Post(fullURL, "application/json", bytes.NewBuffer(jsonBytes))

	if err != nil {
		fmt.Println(err.Error())
	}

	hasService := false
	err = json.NewDecoder(response.Body).Decode(&hasService)
	if err != nil {
		return false
	}

	fmt.Printf("Service %s available on %s, %t\n", svc, node, hasService)
	return hasService
}

func tryAddToNode(ip string, node string, svcName string) bool {
	port := getPort(node)
	ip = getIP(ip)

	fullURL := fmt.Sprintf("http://%s:%d/%s", ip, port, config.Cfg.AddServiceClientURL) //"addServiceClient")

	serviceClient := common.ServiceClient{
		Name:        config.Cfg.NodeID,
		ServiceName: svcName,
	}
	jsonBytes, err := json.Marshal(serviceClient)

	fmt.Printf("Registering with %s as client for %s\n", fullURL, svcName)
	response, err := http.Post(fullURL, "application/json", bytes.NewBuffer(jsonBytes))

	if err != nil {
		fmt.Println(err.Error())
	}

	added := false
	err = json.NewDecoder(response.Body).Decode(&added)

	fmt.Printf("Service added? %t\n", added)
	if err != nil {
		return false
	}
	return added
}

func tryRemoveFromNode(ip string, node string, svcName string) bool {
	port := getPort(node)
	ip = getIP(ip)

	fullURL := fmt.Sprintf("http://%s:%d/%s", ip, port, config.Cfg.RemoveServiceClientURL) // "removeServiceClient")

	serviceClient := common.ServiceClient{
		Name:        config.Cfg.NodeID,
		ServiceName: svcName,
	}
	jsonBytes, err := json.Marshal(serviceClient)
	response, err := http.Post(fullURL, "application/json", bytes.NewBuffer(jsonBytes))

	if err != nil {
		fmt.Println(err.Error())
	}

	removed := false
	err = json.NewDecoder(response.Body).Decode(&removed)
	if err != nil {
		return false
	}
	return removed
}

func notifyMigrateSuccess(ip string, node string, svcName string) bool {
	port := getPort(node)
	ip = getIP(ip)

	fullURL := fmt.Sprintf("http://%s:%d/%s", ip, port, config.Cfg.ConfirmMigrateURL) // "migrateConfirmed")

	serviceClient := common.ServiceClient{
		Name:        config.Cfg.NodeID,
		ServiceName: svcName,
	}
	json, err := json.Marshal(serviceClient)
	_, err = http.Post(fullURL, "application/json", bytes.NewBuffer(json))

	if err != nil {
		fmt.Println(err.Error())
		return false
	}

	return true
}

func notifyMigrateFailed(ip string, node string, svcName string) bool {
	port := getPort(node)
	ip = getIP(ip)
	fullURL := fmt.Sprintf("http://%s:%d/%s", ip, port, config.Cfg.FailedMigrateURL) // "migrateFailed")
	serviceClient := common.ServiceClient{
		Name:        config.Cfg.NodeID,
		ServiceName: svcName,
	}
	json, err := json.Marshal(serviceClient)
	_, err = http.Post(fullURL, "application/json", bytes.NewBuffer(json))

	if err != nil {
		fmt.Println(err.Error())
		return false
	}

	return true
}
