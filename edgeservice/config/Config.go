package config

import (
	"encoding/json"
	"fmt"
	"os"
)

var Cfg *Config

type Config struct {
	Port               int    `json:"port"`
	ServiceMonitorType string `json:"serviceMonitorType"`
	ServiceLocatorType string `json:"serviceLocatorType"`
	//MonitorServices    []string `json:"monitorServices"`
	SupportServices map[string][]string `json:"supportServices"`
	InitialNodes    map[string]string   `json:"initialNodes"`
	MaxPing         float32             `json:"maxPing"`
	NodeID          string              `json:"nodeID"`

	FogPort                int    `json:"fogPort"`
	FetchFogURL            string `json:"fetchFogUrl"`
	FogServiceRunningURL   string `json:"fogServiceRunningUrl"`
	AddServiceClientURL    string `json:"addServiceClientUrl"`
	RemoveServiceClientURL string `json:"removeServiceClientUrl"`
	ConfirmMigrateURL      string `json:"confirmMigrateUrl"`
	FailedMigrateURL       string `json:"failedMigrateUrl"`
	PingPeriod             int    `json:"pingPeriod"`
	PingURL                string `json:"pingUrl"`

	FledgeAPIPort int    `json:"fledgePort"`
	FledgePodURL  string `json:"fledgePodUrl"`

	TestMode             bool    `json:"testMode"`
	FogIP                string  `json:"fogIP"`
	CheatyMinimalFogPing float32 `json:"cheatyMinimalFogPing"`
}

func LoadConfig(filename string) error {
	file, err := os.Open(filename)
	if err != nil {
		//return err
	}
	decoder := json.NewDecoder(file)
	Cfg = &Config{}
	err = decoder.Decode(Cfg)
	if err != nil {
		fmt.Println(err.Error())
		//return err
	}

	fmt.Printf("NodeID check %s\n", Cfg.NodeID)
	if os.Getenv("NODEID") != "" {
		fmt.Printf("Loading nodeID from env instead")
		Cfg.NodeID = os.Getenv("NODEID")
	}
	/*fmt.Printf("SwirlServer check %s\n", Cfg.SwirlServer)
	if os.Getenv("SWIRLSERVER") != "" {
		fmt.Printf("Loading SwirlServer from env instead")
		Cfg.SwirlServer = os.Getenv("SWIRLSERVER")
	}*/

	return err
}
