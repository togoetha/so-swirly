package ws

import (
	"encoding/json"
	"fmt"
	"net/http"
	common "so-swirly/common/algorithm"
	"so-swirly/fogservice/algorithm"
	"so-swirly/fogservice/config"
	"strings"
	"time"
	//"github.com/gorilla/mux"
)

//GET /ping
func Ping(w http.ResponseWriter, r *http.Request) {
	client := common.Client{}

	err := json.NewDecoder(r.Body).Decode(&client)

	fmt.Printf("Ping from %s\n", client.Name)
	if err != nil {
		w.WriteHeader(400)
	} else {
		if client.Type == common.NodeTypeFog {
			ip := strings.Split(r.RemoteAddr, ":")[0]
			go func() {
				common.AddNode(client.Name, ip)
			}()
		}

		if config.Cfg.TestMode {
			sleepTime := float32(0)
			if client.Type == common.NodeTypeFog {
				sleepTime = config.Cfg.CheatyMinimalFogPingMap[client.Name]
			} else {
				sleepTime = config.Cfg.CheatyMinimalEdgePingMap[client.Name]
			}
			fmt.Printf("Sleep time %f for %s\n", sleepTime, client.Name)
			time.Sleep(time.Duration(int(sleepTime)) * time.Millisecond)
		}

		w.Write([]byte("OK"))
		//w.WriteHeader(200)
	}
}

func IsServiceRunning(w http.ResponseWriter, r *http.Request) {
	service := ""

	err := json.NewDecoder(r.Body).Decode(&service)
	fmt.Printf("Is service running %s\n", service)
	if err != nil {
		w.WriteHeader(400)
	} else {
		success := algorithm.HasService(service)
		jsonBytes, _ := json.Marshal(success)
		w.Write(jsonBytes)
	}
}

func AddServiceClient(w http.ResponseWriter, r *http.Request) {
	serviceClient := common.ServiceClient{}

	err := json.NewDecoder(r.Body).Decode(&serviceClient)
	if err != nil {
		w.WriteHeader(400)
	} else {
		ip := strings.Split(r.RemoteAddr, ":")[0]
		client := algorithm.ServiceClient{
			IP:          ip,
			Name:        serviceClient.Name,
			ServiceName: serviceClient.ServiceName,
		}

		fmt.Printf("AddServiceClient %s %s\n", client.Name, client.ServiceName)
		success := algorithm.AddClient(client)
		fmt.Printf("AddServiceClient success? %t\n", success)
		jsonBytes, _ := json.Marshal(success)
		w.Write(jsonBytes)
	}
}

func RemoveServiceClient(w http.ResponseWriter, r *http.Request) {
	serviceClient := common.ServiceClient{}

	err := json.NewDecoder(r.Body).Decode(&serviceClient)
	if err != nil {
		w.WriteHeader(400)
	} else {
		ip := strings.Split(r.RemoteAddr, ":")[0]
		client := algorithm.ServiceClient{
			IP:          ip,
			Name:        serviceClient.Name,
			ServiceName: serviceClient.ServiceName,
		}

		algorithm.RemoveClient(client)
	}
}

func ClientMigrationConfirmed(w http.ResponseWriter, r *http.Request) {
	serviceClient := common.ServiceClient{}

	err := json.NewDecoder(r.Body).Decode(&serviceClient)
	if err != nil {
		w.WriteHeader(400)
	} else {
		ip := strings.Split(r.RemoteAddr, ":")[0]
		client := algorithm.ServiceClient{
			IP:          ip,
			Name:        serviceClient.Name,
			ServiceName: serviceClient.ServiceName,
		}

		algorithm.MigrationConfirmed(client)
	}
}

func ClientMigrationDenied(w http.ResponseWriter, r *http.Request) {
	serviceClient := common.ServiceClient{}

	err := json.NewDecoder(r.Body).Decode(&serviceClient)
	if err != nil {
		w.WriteHeader(400)
	} else {
		ip := strings.Split(r.RemoteAddr, ":")[0]
		client := algorithm.ServiceClient{
			IP:          ip,
			Name:        serviceClient.Name,
			ServiceName: serviceClient.ServiceName,
		}

		algorithm.MigrationDenied(client)
	}
}

func GetKnownFogNodes(w http.ResponseWriter, r *http.Request) {
	nodes := common.GetKnownNodes()
	json, err := json.Marshal(nodes)
	fmt.Printf("Responding %s\n", string(json))
	if err != nil {
		http.Error(w, err.Error(), http.StatusBadRequest)
		return
	}
	w.Write(json)
}

func GetDiscoveredNodeStats(w http.ResponseWriter, r *http.Request) {
	nodes := common.GetKnownNodes()
	cheatNodes := config.Cfg.CheatyMinimalFogPingMap
	maxPing := config.Cfg.MaxPing

	numInRange := 0
	numTolerance := 0
	numOutRange := 0
	numExpected := 0
	discovered := 0

	toDiscover := make(map[string]int)
	for node, ping := range cheatNodes {
		if ping <= maxPing {
			numExpected++
			toDiscover[node] = 1
		}
	}

	for _, node := range nodes {
		if node.Distance <= maxPing+1 {
			numInRange++
		} else if node.Distance <= 2*maxPing {
			numTolerance++
		} else {
			numOutRange++
		}

		_, shouldInRange := toDiscover[node.Name]
		if shouldInRange {
			discovered++
		}
	}

	stats := common.DiscoveredNodes{
		NodesWithinRange:       numInRange,
		NodesInAcceptableRange: numTolerance,
		ExpectedInRange:        numExpected,
		OutsideRange:           numOutRange,
		Discovered:             discovered,
	}
	json, err := json.Marshal(stats)
	fmt.Printf("Responding %s\n", string(json))
	if err != nil {
		http.Error(w, err.Error(), http.StatusBadRequest)
		return
	}
	w.Write(json)
}
