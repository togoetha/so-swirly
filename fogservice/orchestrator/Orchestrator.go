package orchestrator

import (
	v1 "k8s.io/api/core/v1"
)

var Orch Orchestrator

type Orchestrator interface {
	Init() Orchestrator
	DeployPod(pod *v1.Pod) bool
	RemovePod(name string) bool
}
