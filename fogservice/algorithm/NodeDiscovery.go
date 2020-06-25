package algorithm

import (
	"fmt"
	"math"
	"so-swirly/common/algorithm"
	"so-swirly/fogservice/config"
	"strconv"
)

type FogNodePinger struct {
}

func (fp *FogNodePinger) Init() algorithm.NodePinger {
	return fp
}

func (fp *FogNodePinger) GetNodeType() algorithm.NodeType {
	return algorithm.NodeTypeFog
}

func (fp *FogNodePinger) GetPingThreshold(nearbyNodes int) float32 {
	relativeDensity := 10 / float64(nearbyNodes)

	multi := float64(1)
	if relativeDensity > 1 {
		multi = math.Sqrt(relativeDensity)
	}
	fmt.Printf("Ping multiplier %f", multi)

	return config.Cfg.MaxPing * float32(multi)
}

func (fp *FogNodePinger) ShouldReping(node algorithm.FogNode) bool {
	return true
}

func (fp *FogNodePinger) GetFogURL(ip string, node string) string {
	port := getPort(node)
	fullURL := fmt.Sprintf("http://%s:%d/%s", ip, port, "getKnownFogNodes")
	return fullURL
}

func (fp *FogNodePinger) GetPingURL(ip string, node string) string {
	port := getPort(node)
	fullURL := fmt.Sprintf("http://%s:%d/%s", ip, port, "ping")
	return fullURL
}

func (fp *FogNodePinger) OrderKnownNodes(nodes []algorithm.FogNode) []algorithm.FogNode {
	return nodes
}

func (fp *FogNodePinger) GetNodeID() string {
	return config.Cfg.NodeID
}

func getPort(node string) int {
	port := config.Cfg.Port
	if config.Cfg.TestMode {
		nodenumber, _ := strconv.Atoi(node[1:])
		port += nodenumber
	}
	return port
}
