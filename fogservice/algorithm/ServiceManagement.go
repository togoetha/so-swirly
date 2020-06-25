package algorithm

import (
	"bytes"
	"encoding/json"
	"fmt"
	"io/ioutil"
	"net/http"
	"so-swirly/fogservice/config"
	"so-swirly/fogservice/orchestrator"
	"strconv"

	v1 "k8s.io/api/core/v1"
)

type ServiceClient struct {
	Name        string
	IP          string
	ServiceName string
	Migrateable bool
}

type ServiceSpec struct {
	Name       string
	Deleting   bool
	MinClients int
	MaxClients int
}

var clients map[string][]*ServiceClient
var services map[string]*ServiceSpec

func Init() {
	clients = make(map[string][]*ServiceClient)
	services = make(map[string]*ServiceSpec)
}

func HasService(service string) bool {
	spec, exists := services[service]
	return exists && !spec.Deleting
}

func AddClient(client ServiceClient) bool {
	_, present := clients[client.ServiceName]

	if !present {
		success := loadRequiredService(client)
		if !success {
			return false
		}
	}
	return addServiceClient(client)
}

func RemoveClient(client ServiceClient) {
	delete(clients, client.ServiceName)
	//do lower bound clients/resource check
	//not doable on resources obviously, unless a single process is monitored
	//maybe include lower bound on #clients?
	if len(clients[client.ServiceName]) < services[client.ServiceName].MinClients {
		services[client.ServiceName].Deleting = true

		for _, nclient := range clients[client.ServiceName] {
			nclient.Migrateable = false
			notifyTeardown(nclient.IP, nclient.Name, nclient.ServiceName)
		}
	}
}

//braindump: the addclient and removeclient mechanism can also be used for an edgenode to assure that it can join/has joined another service provider before
//calling this method, and removeclient easily reverses addclient with no adverse effects (except maybe a migration cascade?)
//find something to avoid that cascade, maybe a flag for removeclient to indicate it was a temporary thing
func MigrationConfirmed(client ServiceClient) {
	if services[client.ServiceName].Deleting {
		//update list of migrated clients
		client.Migrateable = true
		//if all {
		allMigrated := false
		for _, nclient := range clients[client.ServiceName] {
			if !nclient.Migrateable {
				allMigrated = false
			}
		}
		//if ok, migrate
		if allMigrated {
			for _, nclient := range clients[client.ServiceName] {
				notifyTeardown(nclient.IP, nclient.Name, client.ServiceName)
			}
			delete(clients, client.ServiceName)
			delete(services, client.ServiceName)
			orchestrator.Orch.RemovePod(client.ServiceName)
		}
	}
}

func MigrationDenied(client ServiceClient) {
	services[client.ServiceName].Deleting = false

	for _, nclient := range clients[client.ServiceName] {
		nclient.Migrateable = false
	}

	for _, nclient := range clients[client.ServiceName] {
		cancelTeardown(nclient.IP, nclient.Name, client.ServiceName)
	}
}

func loadRequiredService(client ServiceClient) bool {
	fmt.Printf("LoadRequiredService %s\n", client.ServiceName)
	//load pod yaml for this service
	pod, _ := getPodSpec(client.ServiceName)
	//check resource requirements
	resources := getRequiredResources(pod)

	if resourcesFree(resources) {
		//start service
		fmt.Printf("Service %s resources ok, deploying pod\n", client.ServiceName)
		success := orchestrator.Orch.DeployPod(pod)

		fmt.Printf("Service %s deployed %t\n", client.ServiceName, success)
		min, _ := strconv.Atoi(pod.ObjectMeta.Labels["minClients"])
		max, _ := strconv.Atoi(pod.ObjectMeta.Labels["maxClients"])
		spec := ServiceSpec{
			Name:       client.ServiceName,
			Deleting:   false,
			MinClients: min,
			MaxClients: max,
		}
		services[spec.Name] = &spec
		//register client

		if success {
			clients[client.ServiceName] = []*ServiceClient{} //client}
		}
		return success
		//return success
	} else {
		//server full, deny
		fmt.Printf("Service %s resource check failed\n", client.ServiceName)
		return false
	}
}

func addServiceClient(client ServiceClient) bool {
	resources := make(map[Resource]int)
	resources[CPUShares] = 5
	resources[Memory] = 5
	if resourcesFree(resources) && len(clients[client.ServiceName]) < services[client.ServiceName].MaxClients {
		//just add, nothing more required
		clients[client.ServiceName] = append(clients[client.ServiceName], &client)
		return true
	} else {
		//server full, deny
		return false
	}
}

func notifyInitTeardown(ip string, node string, service string) error {
	port := getEdgePort(node)
	fullURL := fmt.Sprintf("http://%s:%d/%s", ip, port, config.Cfg.EdgeTryMigrateURL)
	fmt.Printf("Calling %s\n", fullURL)
	response, err := http.Post(fullURL, "application/json", bytes.NewBuffer([]byte(service)))

	if response.StatusCode != 200 || err != nil {
		fmt.Println(err.Error())
	}

	response.Body.Close()
	return err
}

func notifyTeardown(ip string, node string, service string) error {
	port := getEdgePort(node)
	fullURL := fmt.Sprintf("http://%s:%d/%s", ip, port, config.Cfg.EdgeMigrateURL)
	fmt.Printf("Calling %s\n", fullURL)
	response, err := http.Post(fullURL, "application/json", bytes.NewBuffer([]byte(service)))

	if response.StatusCode != 200 || err != nil {
		fmt.Println(err.Error())
	}

	response.Body.Close()
	return err
}

func cancelTeardown(ip string, node string, service string) error {
	port := getEdgePort(node)
	fullURL := fmt.Sprintf("http://%s:%d/%s", ip, port, config.Cfg.EdgeCancelMigrateURL)
	fmt.Printf("Calling %s\n", fullURL)
	response, err := http.Post(fullURL, "application/json", bytes.NewBuffer([]byte(service)))

	if response.StatusCode != 200 || err != nil {
		fmt.Println(err.Error())
	}

	response.Body.Close()
	return err
}

func getEdgePort(node string) int {
	port := config.Cfg.EdgePort
	if config.Cfg.TestMode {
		nodenumber, _ := strconv.Atoi(node[1:])
		port += nodenumber
	}
	return port
}

func getPodSpec(name string) (*v1.Pod, error) {
	podBytes, err := ioutil.ReadFile(fmt.Sprintf("services/%s.json", name))
	pod := &(v1.Pod{})

	if err != nil {
		return nil, err
	}

	json.Unmarshal(podBytes, pod)
	return pod, nil
}

func getRequiredResources(pod *v1.Pod) map[Resource]int {
	resources := make(map[Resource]int)
	resources[CPUShares] = 0
	resources[Memory] = 0
	for _, dc := range pod.Spec.Containers {
		if dc.Resources.Limits == nil {
			dc.Resources.Limits = v1.ResourceList{}
		}
		if dc.Resources.Requests == nil {
			dc.Resources.Requests = v1.ResourceList{}
		}
		memory := dc.Resources.Limits.Memory()
		if memory.IsZero() {
			memory = dc.Resources.Requests.Memory()
		}
		if !memory.IsZero() {
			resources[Memory] += int(memory.Value())
		}

		cpu := dc.Resources.Limits.Cpu()
		if cpu.IsZero() {
			cpu = dc.Resources.Requests.Cpu()
		}
		if !cpu.IsZero() {
			resources[CPUShares] += int(cpu.MilliValue())
		}
	}
	return resources
}
