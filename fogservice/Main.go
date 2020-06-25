package main

import (
	"context"
	"fmt"
	"net/http"
	"os"
	common "so-swirly/common/algorithm"
	"so-swirly/fogservice/algorithm"
	"so-swirly/fogservice/config"
	"so-swirly/fogservice/orchestrator"
	"so-swirly/fogservice/ws"
	"strconv"
)

var kubernetesHost string
var kubernetesPort string
var defaultPodFile string

var rootContext, rootContextCancel = context.WithCancel(context.Background())

var pings map[string]map[string]int

func main() {
	argsWithoutProg := os.Args[1:]
	cfgFile := "fog1.json"
	if len(argsWithoutProg) > 0 {
		cfgFile = argsWithoutProg[0]
	}

	config.LoadConfig(cfgFile)

	//go func() { startUpdateResources() }()

	/*file, _ := os.Open("supportservice1.json")
	decoder := json.NewDecoder(file)
	pod := &v1.Pod{}
	decoder.Decode(pod)

	labels := make(map[string]string)
	labels["minClients"] = "0"
	labels["maxClients"] = "1000"
	pod.ObjectMeta.Labels = labels

	jsonBytes, _ := json.Marshal(pod)
	fmt.Println(string(jsonBytes))*/

	algorithm.Init()
	orchType := config.Cfg.Orchestrator
	switch orchType {
	case "fledge":
		orchestrator.Orch = (&(orchestrator.FledgeOrchestrator{})).Init()
	}

	go func() {
		common.StartDiscovery(&algorithm.FogNodePinger{}, config.Cfg.PingPeriod, config.Cfg.InitialNodes)
	}()

	defer func() {
		if r := recover(); r != nil {
			fmt.Println("Recovered in main.go", r)
		}
	}()

	router := ws.FogRouter()
	port := config.Cfg.Port
	if config.Cfg.TestMode {
		nodeNr, _ := strconv.Atoi(config.Cfg.NodeID[1:])
		port += nodeNr
	}
	fmt.Printf("Hosting node %s on port %d\n", config.Cfg.NodeID, port)
	err := http.ListenAndServe(fmt.Sprintf(":%d", port), router)
	if err != nil {
		fmt.Println(err.Error())
	}
}
