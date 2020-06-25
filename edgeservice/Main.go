package main

import (
	"fmt"
	"net/http"
	"os"
	common "so-swirly/common/algorithm"
	"so-swirly/edgeservice/algorithm"
	"so-swirly/edgeservice/config"
	"so-swirly/edgeservice/monitor"
	"so-swirly/edgeservice/ws"
	"strconv"
)

func main() {
	argsWithoutProg := os.Args[1:]
	cfgFile := "defaultconfig.json"
	if len(argsWithoutProg) > 0 {
		cfgFile = argsWithoutProg[0]
	}

	config.LoadConfig(cfgFile)

	/*inodes := make(map[string]string)
	inodes["node1"] = "192.168.1.100"
	inodes["node2"] = "192.168.1.101"
	svcs := make(map[string][]string)
	svcs["testservice"] = []string{"fogservice1", "fogservice2"}
	testCfg := config.Config{
		Port:                   8181,
		ServiceMonitorType:     "fledge",
		ServiceLocatorType:     "hosts",
		SupportServices:        svcs,
		FogPort:                8182,
		FetchFogURL:            "getKnownFogNodes",
		FogServiceRunningURL:   "isServiceRunning",
		AddServiceClientURL:    "addServiceClient",
		RemoveServiceClientURL: "removeServiceClient",
		ConfirmMigrateURL:      "migrateConfirmed",
		FailedMigrateURL:       "migrateFailed",
		PingURL:                "ping",
		FledgeAPIPort:          12345,
		FledgePodURL:           "getPods",
		NodeID:                 "randomnode",
		PingPeriod:             60,
		MaxPing:                100,
	}
	json, _ := json.Marshal(testCfg)
	fmt.Println(string(json))*/

	/*sig := make(chan os.Signal, 1)
	signal.Notify(sig, syscall.SIGINT, syscall.SIGTERM)
	go func() {
		<-sig
		//rootContextCancel()
		common.Stop()
	}()*/

	algorithm.InitMesher()

	go func() {
		common.StartDiscovery(&algorithm.EdgeNodePinger{}, config.Cfg.PingPeriod, config.Cfg.InitialNodes)
	}()

	go func() {
		svcMap := config.Cfg.SupportServices
		toMonitor := []string{}
		for svc, _ := range svcMap {
			toMonitor = append(toMonitor, svc)
		}

		orchType := config.Cfg.ServiceMonitorType
		switch orchType {
		case "fledge":
			monitor.Monitor = &monitor.FledgeServiceMonitor{}
		default:
			monitor.Monitor = &monitor.FledgeServiceMonitor{}
		}

		monitor.Monitor.ServiceDeployedCallback(algorithm.DeploySupportServicesFor)
		monitor.Monitor.ServiceRemovedCallback(algorithm.RemoveSupportServicesFor)
		monitor.Monitor.Init(toMonitor)
	}()

	orchType := config.Cfg.ServiceLocatorType
	switch orchType {
	case "hosts":
		algorithm.Locator = (&algorithm.HostsServiceLocator{}).Init()
	default:
		algorithm.Locator = (&algorithm.HostsServiceLocator{}).Init()
	}

	router := ws.EdgeRouter()
	port := config.Cfg.Port
	if config.Cfg.TestMode {
		nodeNr, _ := strconv.Atoi(config.Cfg.NodeID[1:])
		port += nodeNr
	}

	err := http.ListenAndServe(fmt.Sprintf(":%d", port), router)
	if err != nil {
		fmt.Println(err.Error())
	}
}
