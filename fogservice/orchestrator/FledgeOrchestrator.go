package orchestrator

import (
	"bytes"
	"encoding/json"
	"fmt"
	"net/http"
	"so-swirly/fogservice/config"

	v1 "k8s.io/api/core/v1"
)

type FledgeOrchestrator struct {
}

func (fo *FledgeOrchestrator) Init() Orchestrator {
	//fo.clientset = getKubeClient()
	return fo
}

func (fo *FledgeOrchestrator) DeployPod(pod *v1.Pod) bool {
	fullURL := fmt.Sprintf("http://localhost:%d/deployPod", config.Cfg.FledgeAPIPort)
	fmt.Printf("Calling %s\n", fullURL)

	if config.Cfg.TestMode {
		return true
	}

	json, err := json.Marshal(pod)
	if err != nil {
		//http.Error(w, err.Error(), http.StatusBadRequest)
		return false
	}
	response, err := http.Post(fullURL, "application/json", bytes.NewBuffer(json))
	if err != nil {

	}
	defer response.Body.Close()
	return err == nil && response.StatusCode == http.StatusOK
}

func (fo *FledgeOrchestrator) RemovePod(name string) bool {
	fullURL := fmt.Sprintf("http://localhost:%d/deletePod", config.Cfg.FledgeAPIPort)
	fmt.Printf("Calling %s\n", fullURL)

	if config.Cfg.TestMode {
		return true
	}

	response, err := http.Post(fullURL, "application/json", bytes.NewBuffer([]byte(name)))
	if err != nil {

	}
	defer response.Body.Close()
	return err == nil && response.StatusCode == http.StatusOK
}
