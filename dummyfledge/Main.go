package main

import (
	"context"
	"fmt"
	"net/http"
	"os"
	"so-swirly/dummyfledge/config"
	"so-swirly/dummyfledge/ws"
)

var kubernetesHost string
var kubernetesPort string
var defaultPodFile string

var rootContext, rootContextCancel = context.WithCancel(context.Background())

var pings map[string]map[string]int

func main() {
	argsWithoutProg := os.Args[1:]
	cfgFile := "defaultconfig.json"
	if len(argsWithoutProg) > 0 {
		cfgFile = argsWithoutProg[0]
	}

	config.LoadConfig(cfgFile)

	/*meta := metav1.ObjectMeta{
		Name: "fogservice1",
	}

	hMeg, _ := resource.ParseQuantity("100Mi")
	unit, _ := resource.ParseQuantity("1")
	requests := make(map[v1.ResourceName]resource.Quantity)
	requests[v1.ResourceCPU] = unit
	requests[v1.ResourceMemory] = hMeg
	requests[v1.ResourceStorage] = hMeg

	limits := make(map[v1.ResourceName]resource.Quantity)
	limits[v1.ResourceCPU] = unit
	limits[v1.ResourceMemory] = hMeg
	limits[v1.ResourceStorage] = hMeg

	resources := v1.ResourceRequirements{
		Requests: requests,
		Limits:   limits,
	}
	c := v1.Container{
		Resources: resources,
	}
	containers := []v1.Container{c}
	spec := v1.PodSpec{
		Containers: containers,
	}
	pod := v1.Pod{
		ObjectMeta: meta,
		Spec:       spec,
	}

	jsonbytes, _ := json.Marshal(pod)
	fmt.Println(string(jsonbytes))
	*/
	router := ws.FogRouter()
	port := config.Cfg.Port
	/*if config.Cfg.TestMode {
		nodeNr, _ := strconv.Atoi(config.Cfg.NodeID[1:])
		port += nodeNr
	}*/
	fmt.Printf("Hosting on port %d\n", port)
	err := http.ListenAndServe(fmt.Sprintf(":%d", port), router)
	if err != nil {
		fmt.Println(err.Error())
	}
}
