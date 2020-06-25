package config

import (
	"encoding/json"
	"fmt"
	"os"
)

var Cfg *Config

type Config struct {
	MinEdgeNodes  int    `json:"minEdgeNodes"`
	MaxEdgeNodes  int    `json:"maxEdgeNodes"`
	EdgeNodeStep  int    `json:"edgeNodeStep"`
	MinFogNodes   int    `json:"minFogNodes"`
	MaxFogNodes   int    `json:"maxFogNodes"`
	FogNodeStep   int    `json:"fogNodeStep"`
	Iterations    int    `json:"iterations"`
	MonitorPeriod int    `json:"monitorPeriod"`
	MonitorLoops  int    `json:"monitorLoops"`
	EthInterface  string `json:"ethInterface"`
}

func LoadConfig(filename string) error {
	//fmt.Printf("Loading config %s\n", filename)
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

	return err
}
