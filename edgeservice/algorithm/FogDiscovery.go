package algorithm

import (
	"fmt"
	"math"
	"so-swirly/common/algorithm"
	"so-swirly/edgeservice/config"
	"sort"
	"strconv"
)

type EdgeNodePinger struct {
}

func (fp *EdgeNodePinger) Init() algorithm.NodePinger {
	return fp
}

func (fp *EdgeNodePinger) GetNodeType() algorithm.NodeType {
	return algorithm.NodeTypeEdge
}

func (fp *EdgeNodePinger) GetPingThreshold(nearbyNodes int) float32 {
	relativeDensity := 10 / float64(nearbyNodes)

	multi := float64(1)
	if relativeDensity > 1 {
		multi = math.Sqrt(relativeDensity)
	}
	fmt.Printf("Ping multiplier %f\n", multi)

	return config.Cfg.MaxPing * float32(multi)
}

func (fp *EdgeNodePinger) ShouldReping(node algorithm.FogNode) bool {
	return true
}

func (fp *EdgeNodePinger) GetFogURL(ip string, node string) string {
	port := getPort(node)
	ip = getIP(ip)
	fullURL := fmt.Sprintf("http://%s:%d/%s", ip, port, config.Cfg.FetchFogURL)
	return fullURL
}

func (fp *EdgeNodePinger) GetPingURL(ip string, node string) string {
	port := getPort(node)
	ip = getIP(ip)
	fullURL := fmt.Sprintf("http://%s:%d/%s", ip, port, config.Cfg.PingURL)
	return fullURL
}

func (fp *EdgeNodePinger) GetNodeID() string {
	return config.Cfg.NodeID
}

func (fp *EdgeNodePinger) OrderKnownNodes(nodes []algorithm.FogNode) []algorithm.FogNode {
	return SortNodePings(nodes)
}

func SortNodePings(pings []algorithm.FogNode) []algorithm.FogNode {
	nodePings := []algorithm.FogNode{}
	for _, ping := range pings {
		//nPing := NodePing{Node: fn, Ping: ping}
		nodePings = append(nodePings, ping)
	}

	ping := func(p1, p2 *algorithm.FogNode) bool {
		return p1.Distance < p2.Distance
	}
	By(ping).Sort(nodePings)

	return nodePings
}

type By func(p1, p2 *algorithm.FogNode) bool

// Sort is a method on the function type, By, that sorts the argument slice according to the function.
func (by By) Sort(pings []algorithm.FogNode) {
	ps := &pingSorter{
		pings: pings,
		by:    by, // The Sort method's receiver is the function (closure) that defines the sort order.
	}
	sort.Sort(ps)
}

type pingSorter struct {
	pings []algorithm.FogNode
	by    func(p1, p2 *algorithm.FogNode) bool // Closure used in the Less method.
}

// Len is part of sort.Interface.
func (s *pingSorter) Len() int {
	return len(s.pings)
}

// Swap is part of sort.Interface.
func (s *pingSorter) Swap(i, j int) {
	s.pings[i], s.pings[j] = s.pings[j], s.pings[i]
}

// Less is part of sort.Interface. It is implemented by calling the "by" closure in the sorter.
func (s *pingSorter) Less(i, j int) bool {
	return s.by(&s.pings[i], &s.pings[j])
}

func getPort(node string) int {
	port := config.Cfg.FogPort
	if config.Cfg.TestMode {
		nodenumber, _ := strconv.Atoi(node[1:])
		port += nodenumber
	}
	return port
}

func getIP(ip string) string {
	if config.Cfg.TestMode {
		return config.Cfg.FogIP
	} else {
		return ip
	}
}
