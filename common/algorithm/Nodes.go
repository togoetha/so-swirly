package algorithm

import (
	"bytes"
	"encoding/json"
	"fmt"
	"net/http"

	//"so-swirly/fogservice/config"
	"time"
)

type NodePinger interface {
	Init() NodePinger
	OrderKnownNodes([]FogNode) []FogNode
	GetNodeType() NodeType
	GetPingURL(ip string, node string) string
	GetFogURL(ip string, node string) string
	GetPingThreshold(nearbyNodes int) float32
	ShouldReping(node FogNode) bool
	GetNodeID() string
}

type FogNode struct {
	Name     string
	IP       string
	Distance float32
}

type DiscoveredNodes struct {
	NodesWithinRange       int
	NodesInAcceptableRange int
	ExpectedInRange        int
	OutsideRange           int
	Discovered             int
}

type NodeStats struct {
	CurrentClosestPing float32
	MinimalPing        float32
	CurrentFogNode     string
}

type Client struct {
	Name string
	Type NodeType
}

type ServiceClient struct {
	Name        string
	ServiceName string
}

type NodeType string

var NodeTypeFog NodeType = "fognode"
var NodeTypeEdge NodeType = "edgenode"

var discoveredNodes map[string]FogNode
var lowestPing float32
var active bool
var pingPeriod int
var pinger NodePinger

//var nodeLock sync.Mutex
var nodesToAdd []FogNode

var NodesUpdatedCallback func()

func GetKnownNodes() []FogNode {
	nodes := []FogNode{}

	//nodeLock.Lock()
	//defer nodeLock.Unlock()
	for _, node := range discoveredNodes {
		if node.Distance != -1 {
			nodes = append(nodes, node)
		}
	}
	return pinger.OrderKnownNodes(nodes)
}

func StartDiscovery(p NodePinger, period int, nodes map[string]string) {
	active = true
	pinger = p
	pingPeriod = period
	lowestPing = 1000000
	nodesToAdd = []FogNode{}
	discoveredNodes = make(map[string]FogNode)

	//nodes := make(map[string]string)
	//json.Unmarshal(nodejson, &nodes)
	for node, ip := range nodes {
		discoveredNodes[node] = FogNode{
			Name: node,
			IP:   ip,
		}
	}

	defer func() {
		if r := recover(); r != nil {
			fmt.Println("Recovered in NodeDiscovery", r)
		}
	}()

	//initialDiscover()
	for active {
		fmt.Printf("Ping/discover... Active ? %t\n", active)
		pingAndDiscover()
		if NodesUpdatedCallback != nil {
			go func() {
				NodesUpdatedCallback()
			}()
		}
		time.Sleep(time.Duration(pingPeriod) * time.Second)
	}
}

func initialDiscover() {
	toPing := []FogNode{}
	//tooFar := []FogNode{}

	for _, node := range discoveredNodes {
		toPing = append(toPing, node)
	}

	for _, node := range nodesToAdd {
		toPing = append(toPing, node)
	}
	nodesToAdd = []FogNode{}

	fmt.Println("Start initial discovery")

	curNode := FogNode{}

	for len(toPing) > 0 {
		curNode, toPing = toPing[0], toPing[1:]
		fmt.Printf("Nodes 2 to ping %d\n", len(toPing))
		if pinger.ShouldReping(curNode) {
			curNode.Distance, _ = getPing(pinger.GetNodeType(), pinger.GetPingURL(curNode.IP, curNode.Name))
			if curNode.Distance < lowestPing {
				lowestPing = curNode.Distance
			}
			fmt.Printf("Node %s ping %f\n", curNode.Name, curNode.Distance)

			//determine whether to add to "known" nodes or delete it based on ping
			//if curNode.Distance <= pinger.GetPingThreshold()*5 {
			fmt.Println("Distance ok, updating node")
			discoveredNodes[curNode.Name] = curNode
			newNodes, _ := getKnownFogNodes(pinger.GetFogURL(curNode.IP, curNode.Name))
			toPing = mergeNodes(toPing, newNodes, []FogNode{})
			//nodesWithinReach = true
			/*} /*else {
				fmt.Println("Distance too large")
				tooFar = append(tooFar, curNode)
				//TODO: break the circle of pinging right here, don't let a node back in if we already removed it this ping round
				//if it's the last node remaining or has a lower ping than the others, let it stay anyway, it's the best link around
				if len(discoveredNodes) <= 1 || curNode.Distance < lowestPing {
					fmt.Printf("Not removing node %s due to last one or lowest ping\n", curNode.Name)
					lowestPing = curNode.Distance
					newNodes, _ := getKnownFogNodes(pinger.GetFogURL(curNode.IP, curNode.Name))
					toPing = mergeNodes(toPing, newNodes, tooFar)
					discoveredNodes[curNode.Name] = curNode
				} else {
					_, contains := discoveredNodes[curNode.Name]
					if contains {
						fmt.Printf("Removing %s\n", curNode.Name)
						delete(discoveredNodes, curNode.Name)
					}
				}
			}*/
		}
		fmt.Printf("Nodes 3 to ping %d\n", len(toPing))
	}
	/*if nodesWithinReach {
		tooFar := []string{}
		for name, node := range discoveredNodes {
			if node.Distance > pinger.GetPingThreshold() {
				tooFar = append(tooFar, name)
			}
		}

		for _, name := range tooFar {
			fmt.Printf("Post discovery removing %s because too far\n", name)
			delete(discoveredNodes, name)
		}
	}*/
	fmt.Println("Stop discovery round")
}

func pingAndDiscover() {
	toPing := []FogNode{}
	tooFar := []FogNode{}

	//nodeLock.Lock()
	//defer nodeLock.Unlock()

	for _, node := range discoveredNodes {
		toPing = append(toPing, node)
	}

	for _, node := range nodesToAdd {
		toPing = append(toPing, node)
	}
	nodesToAdd = []FogNode{}

	if len(toPing) > 0 {
		lowestPing, _ = getPing(pinger.GetNodeType(), pinger.GetPingURL(toPing[0].IP, toPing[0].Name))
	}
	fmt.Println("Start discovery round")
	fmt.Printf("Nodes 1 to ping %d\n", len(toPing))
	curNode := FogNode{}
	nodesWithinReach := false
	for len(toPing) > 0 {
		curNode, toPing = toPing[0], toPing[1:]
		//fmt.Printf("Nodes 2 to ping %d\n", len(toPing))
		if pinger.ShouldReping(curNode) {
			curNode.Distance, _ = getPing(pinger.GetNodeType(), pinger.GetPingURL(curNode.IP, curNode.Name))
			fmt.Printf("Node %s ping %f\n", curNode.Name, curNode.Distance)
			/*if curNode.Distance < lowestPing {
				fmt.Printf("Lowest ping reduced to %f\n", lowestPing)
				lowestPing = curNode.Distance
			}*/

			mPing := pinger.GetPingThreshold(len(discoveredNodes))
			//determine whether to add to "known" nodes or delete it based on ping
			if curNode.Distance <= mPing {
				fmt.Printf("Distance %f < %f, updating node\n", curNode.Distance, mPing)
				discoveredNodes[curNode.Name] = curNode
				newNodes, _ := getKnownFogNodes(pinger.GetFogURL(curNode.IP, curNode.Name))
				toPing = mergeNodes(toPing, newNodes, tooFar)
				nodesWithinReach = true

				if curNode.Distance < lowestPing {
					fmt.Printf("Lowest ping reduced to %f\n", lowestPing)
					lowestPing = curNode.Distance
				}
			} else {
				fmt.Printf("Distance %f > %f\n", curNode.Distance, mPing)
				tooFar = append(tooFar, curNode)
				//TODO: break the circle of pinging right here, don't let a node back in if we already removed it this ping round
				//if it's the last node remaining or has a lower ping than the others, let it stay anyway, it's the best link around
				if lowestPing > pinger.GetPingThreshold(len(discoveredNodes)) {
					fmt.Printf("Lowest ping higher than threshold, merging new nodes anyway\n")
					newNodes, _ := getKnownFogNodes(pinger.GetFogURL(curNode.IP, curNode.Name))
					toPing = mergeNodes(toPing, newNodes, tooFar)
				}
				if len(discoveredNodes) <= 1 || curNode.Distance <= lowestPing {
					fmt.Printf("Lowest ping reduced to %f\n", lowestPing)
					fmt.Printf("Not removing node %s due to last one or lowest ping\n", curNode.Name)
					lowestPing = curNode.Distance
					//newNodes, _ := getKnownFogNodes(pinger.GetFogURL(curNode.IP, curNode.Name))
					//toPing = mergeNodes(toPing, newNodes, tooFar)
					discoveredNodes[curNode.Name] = curNode
				} else {
					_, contains := discoveredNodes[curNode.Name]
					if contains {
						fmt.Printf("Removing %s\n", curNode.Name)
						delete(discoveredNodes, curNode.Name)
					}
				}

			}
		}
		//fmt.Printf("Nodes 3 to ping %d\n", len(toPing))
	}
	if nodesWithinReach {
		tooFar := []string{}
		for name, node := range discoveredNodes {
			if node.Distance > pinger.GetPingThreshold(len(discoveredNodes)) {
				tooFar = append(tooFar, name)
			}
		}

		for _, name := range tooFar {
			fmt.Printf("Post discovery removing %s because too far\n", name)
			delete(discoveredNodes, name)
		}
	}
	discoverMap := make(map[string]float32)
	for name, node := range discoveredNodes {
		discoverMap[name] = node.Distance
	}
	jsonBytes, _ := json.Marshal(discoverMap)
	fmt.Printf("Stop discovery round, found %s\n", string(jsonBytes))
}

func getKnownFogNodes(fogURL string) ([]FogNode, error) {

	//fmt.Printf("Fetching fog nodes of %s\n", fullURL)
	response, err := http.Get(fogURL)

	if err != nil {
		fmt.Println(err.Error())
	}

	fogNodes := []FogNode{}
	err = json.NewDecoder(response.Body).Decode(&fogNodes)
	if err != nil {
		return nil, err
	}

	response.Body.Close()
	return fogNodes, nil
}

func mergeNodes(toPing []FogNode, newNodes []FogNode, tooFar []FogNode) []FogNode {
	fmt.Println("Merge nodes")
	for _, node := range newNodes {
		_, discovered := discoveredNodes[node.Name]
		contains := containsElement(node, toPing)
		distant := containsElement(node, tooFar)
		fmt.Printf("Discoverednodes contains %s ? %t\n", node.Name, contains)
		if !contains && !discovered && !distant && node.Name != pinger.GetNodeID() {
			fmt.Printf("Merging node %s\n", node.Name)
			toPing = append(toPing, node)
		}
	}
	return toPing
}

func containsElement(fn FogNode, array []FogNode) bool {
	contains := false
	for _, node := range array {
		if fn.Name == node.Name {
			contains = true
		}
	}

	return contains
}

func AddNode(name string, ip string) {
	fmt.Printf("Add node %s ip %s\n", name, ip)
	if name == pinger.GetNodeID() {
		fmt.Println("Won't add self, return")
		return
	}

	//nodeLock.Lock()
	//defer nodeLock.Unlock()

	_, contains := discoveredNodes[name]
	if !contains {
		fmt.Printf("Unknown node, creating and pinging\n")
		fogNode := FogNode{
			Name:     name,
			IP:       ip,
			Distance: -1,
		}
		//fogNode.Distance = -1

		//discoveredNodes[name] = fogNode
		nodesToAdd = append(nodesToAdd, fogNode)
		fmt.Printf("Added %s ip %s ping %f\n", name, ip, float32(-1))
	} else {
		fmt.Printf("Node %s already known\n", name)
	}
}

func getPing(ntype NodeType, url string) (float32, error) {
	start := time.Now()

	//fullURL := fmt.Sprintf("http://%s:%d/%s", ip, config.Cfg.Port, "ping")

	client := Client{
		Name: pinger.GetNodeID(), //hostname,
		Type: ntype,
	}
	clientJson, _ := json.Marshal(client)

	fmt.Printf("Pinging %s with %s\n", url, string(clientJson))
	response, err := http.Post(url, "application/json", bytes.NewBuffer(clientJson))

	stop := time.Now()

	newPing := float32(stop.Sub(start).Nanoseconds()) / 1000000
	if err != nil {
		fmt.Println(err.Error())
	}
	if response.StatusCode != 200 {
		fmt.Printf("Status code %d", response.StatusCode)
		newPing = -1
	}

	response.Body.Close()
	return newPing, err
}

func Stop() {
	fmt.Println("Stopping discovery service")
	active = false
}
