package config

import (
	"encoding/json"
	"fmt"
	"os"
)

var Cfg *Config

type Config struct {
	Port                     int                `json:"port"`
	Orchestrator             string             `json:"orchestrator"`
	NodeID                   string             `json:"nodeID"`
	ResourceLimitsPct        int                `json:"resourceLimitsPct"`
	InitialNodes             map[string]string  `json:"initialNodes"`
	PingPeriod               int                `json:"pingPeriod"`
	MaxPing                  float32            `json:"maxPing"`
	FledgeAPIPort            int                `json:"fledgeAPIPort"`
	EdgePort                 int                `json:"edgePort"`
	EdgeTryMigrateURL        string             `json:"edgeTryMigrateURL"`
	EdgeMigrateURL           string             `json:"edgeMigrateURL"`
	EdgeCancelMigrateURL     string             `json:"edgeCancelMigrateURL"`
	CheatyMinimalFogPingMap  map[string]float32 `json:"cheatyMinimalFogPingMap"`
	CheatyMinimalEdgePingMap map[string]float32 `json:"cheatyMinimalEdgePingMap"`
	TestMode                 bool               `json:"testMode"`
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
