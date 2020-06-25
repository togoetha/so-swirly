package ws

import (
	"encoding/json"
	"fmt"
	"net/http"
	common "so-swirly/common/algorithm"
	"so-swirly/edgeservice/algorithm"
	"so-swirly/edgeservice/config"
	//"github.com/gorilla/mux"
)

//GET /setFogNodes
func TryMigrate(w http.ResponseWriter, r *http.Request) {
	go func() {
		fmt.Println("TryMigrate")

		service := ""
		err := json.NewDecoder(r.Body).Decode(&service)
		if err != nil {
			http.Error(w, err.Error(), http.StatusBadRequest)
			return
		}

		algorithm.TryMigrateService(service)
	}()
}

func Migrate(w http.ResponseWriter, r *http.Request) {
	fmt.Println("Migrate")

	service := ""
	err := json.NewDecoder(r.Body).Decode(&service)
	if err != nil {
		http.Error(w, err.Error(), http.StatusBadRequest)
		return
	}

	algorithm.MigrateService(service)
}

func CancelMigrate(w http.ResponseWriter, r *http.Request) {
	fmt.Println("CancelMigrate")

	service := ""
	err := json.NewDecoder(r.Body).Decode(&service)
	if err != nil {
		http.Error(w, err.Error(), http.StatusBadRequest)
		return
	}

	algorithm.CancelMigrate(service)
}

func GetNodeStats(w http.ResponseWriter, r *http.Request) {
	//nodes := common.GetKnownNodes()
	services := algorithm.GetFogServices()
	bestPing := config.Cfg.CheatyMinimalFogPing

	bestCurrentPing := float32(0)
	currentFog := ""
	/*if len(nodes) > 0 {
		bestCurrentPing = nodes[0].Distance
	}*/
	node, found := services["supportservice1"]
	if found {
		bestCurrentPing = node.Distance
		currentFog = node.Name
	}

	stats := common.NodeStats{
		CurrentClosestPing: bestCurrentPing,
		MinimalPing:        bestPing,
		CurrentFogNode:     currentFog,
	}

	json, err := json.Marshal(stats)
	fmt.Printf("Responding %s\n", string(json))
	if err != nil {
		http.Error(w, err.Error(), http.StatusBadRequest)
		return
	}
	w.Write(json)
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
