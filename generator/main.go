package main

import (
	"bufio"
	"encoding/json"
	"fmt"
	"image"
	"image/color"
	"image/draw"
	"image/png"
	_ "image/png"
	"io/ioutil"
	"math"
	"math/rand"
	"os"
	edgeconfig "so-swirly/edgeservice/config"
	fogconfig "so-swirly/fogservice/config"
	"so-swirly/generator/algorithm"
	"so-swirly/generator/config"
)

func main() {
	argsWithoutProg := os.Args[1:]
	cfgFile := "defaultconfig.json"
	if len(argsWithoutProg) > 0 {
		cfgFile = argsWithoutProg[0]
	}

	//fmt.Printf("Loading config file %s\n", cfgFile)
	config.LoadConfig(cfgFile)

	//determine what type of test needs to be done. It's best to do only one of these at a time, but in theory they can all run in sequence.
	//if config.Cfg.SpeedTest {
	BuildConfigs()
	//}
}

//Rounds a number up to a multiple of FogNodeStep, required for finding the minimum number of fog nodes for an edge infrastructure and then
//making it adhere to the test settings
func RoundUp(number float64) int {
	steps := number / float64(config.Cfg.FogNodeStep)
	return config.Cfg.FogNodeStep * int(math.Ceil(steps))
}

//This builds a service topology and measures the timings of the add and delete operations.
//For automation purposes, it iterates from MinEdgeNodes to MaxEdgeNodes in EdgeNodeStep steps.
//The same goes for MinFogNodes, MaxFogNodes and FogNodeStep, however MinFogNodes can be overriden by the magic number 800 (see below).
//Because the generated fog nodes and edge nodes are completely random, any topology may turn out to be wildly positive or negative, skewing the results.
//Therefore, it is recommended to do a good number of runs per edgenode/fognode step (Iterations = 20 seems good).
//TODO: get rid of the magic number "800": it has to do with how many clients can "safely" fit on the average fog node given the hardcoded
//resource constraints in clustering.go.
//In other words, if we take more than 800, the algorithm will GIGO because it can't find a spot for every edge node, even a bad one.
func BuildConfigs() {
	//slaMaxPing := float32(config.Cfg.SLAMaxPing)
	maxPingDiff := float64(config.Cfg.MaxPingDiff)
	file, err := os.Open(config.Cfg.DensityMap)
	if err != nil {
		fmt.Printf("Couldn't open %s", config.Cfg.DensityMap)
		return
	}
	defer file.Close()
	densityMapPng, _, err := image.Decode(file)
	hslDMap := hslDensityMap(densityMapPng)

	//iterate over number of edge nodes
	for en := config.Cfg.MinEdgeNodes; en <= config.Cfg.MaxEdgeNodes; en += config.Cfg.EdgeNodeStep {

		//iterate over number of fog nodes, start at a minimum of 1 per 800 edge nodes
		fnLimit := float64(RoundUp(float64(en) / 1000))
		for fn := int(math.Max(fnLimit, float64(config.Cfg.MinFogNodes))); fn <= config.Cfg.MaxFogNodes; fn += config.Cfg.FogNodeStep {
			for iter := 0; iter < config.Cfg.Iterations; iter++ {
				algorithm.GenerateNodes(densityMapPng, en, fn, maxPingDiff)

				imgTopo := image.NewRGBA(hslDMap.Bounds())
				draw.Draw(imgTopo, imgTopo.Bounds(), hslDMap, image.Point{0, 0}, draw.Src)

				fogPath := fmt.Sprintf("fog/f%d-e%d/it%d", fn, en, iter)
				edgePath := fmt.Sprintf("edge/f%d-e%d/it%d", fn, en, iter)
				os.MkdirAll(fogPath, os.ModePerm)
				os.MkdirAll(edgePath, os.ModePerm)

				knownNodes := make(map[string]string)
				//staticKnownNodes := make(map[string]string)
				//staticKnownNodes["f0"] = "127.0.0.1"
				for counter := 0; counter < len(algorithm.FogNodes); counter++ { // _, fogNode := range algorithm.FogNodes {
					fogNode := algorithm.FogNodes[counter]
					cheatyPings := make(map[string]float32)
					for fn, ping := range fogNode.Pings {
						cheatyPings[fn] = ping
					}

					cheatyEdgePings := make(map[string]float32)
					for _, en := range algorithm.EdgeNodes {
						cheatyEdgePings[en.Name] = en.Pings[fogNode.Name]
					}

					fnCfg := fogconfig.Config{
						Port:                     10000,
						Orchestrator:             "fledge",
						NodeID:                   fogNode.Name, //fmt.Sprintf("f%d", counter),
						ResourceLimitsPct:        80,
						InitialNodes:             pickKnownNodes(knownNodes),
						PingPeriod:               10,
						MaxPing:                  float32(config.Cfg.SLAMaxPing),
						FledgeAPIPort:            20000,
						EdgePort:                 12000,
						EdgeTryMigrateURL:        "tryMigrate",
						EdgeMigrateURL:           "migrate",
						EdgeCancelMigrateURL:     "cancelMigrate",
						CheatyMinimalFogPingMap:  cheatyPings,
						CheatyMinimalEdgePingMap: cheatyEdgePings,
						TestMode:                 true,
					}

					cfgBytes, err := json.Marshal(fnCfg)
					err = ioutil.WriteFile(fmt.Sprintf("%s/fog%d.json", fogPath, counter), cfgBytes, 0644)
					if err != nil {
						//panic(err)
						fmt.Println(err.Error())
					}

					knownNodes[fogNode.Name] = "127.0.0.1"

					drawPoint(imgTopo, image.Point{X: int(fogNode.X), Y: int(fogNode.Y)}, 7, color.RGBA{255, 0, 0, 255})
				}

				fogBytes, err := json.Marshal(algorithm.FogNodes)
				if err != nil {
					//panic(err)
					fmt.Println(err.Error())
				}
				err = ioutil.WriteFile(fmt.Sprintf("%s/fog.json", fogPath), fogBytes, 0644)
				if err != nil {
					//panic(err)
					fmt.Println(err.Error())
				}

				services := make(map[string][]string)
				services["monitorservice1"] = []string{"supportservice1"}
				for counter := 0; counter < len(algorithm.EdgeNodes); counter++ { //for _, edgeNode := range algorithm.EdgeNodes {
					edgeNode := algorithm.EdgeNodes[counter]

					closest := float32(100000)
					for _, node := range algorithm.FogNodes {
						dist := edgeNode.Pings[node.Name]
						if dist < closest {
							closest = dist
						}
					}

					enCfg := edgeconfig.Config{
						Port:                   12000,
						ServiceMonitorType:     "fledge",
						ServiceLocatorType:     "hosts",
						SupportServices:        services,
						InitialNodes:           pickKnownNodes(knownNodes),
						MaxPing:                float32(config.Cfg.SLAMaxPing),
						NodeID:                 edgeNode.Name, //fmt.Sprintf("f%d", counter),
						FogPort:                10000,
						FetchFogURL:            "getKnownFogNodes",
						FogServiceRunningURL:   "isServiceRunning",
						AddServiceClientURL:    "addServiceClient",
						RemoveServiceClientURL: "removeServiceClient",
						ConfirmMigrateURL:      "migrateConfirmed",
						FailedMigrateURL:       "migrateFailed",
						PingPeriod:             10,
						PingURL:                "ping",
						FledgeAPIPort:          12345,
						FledgePodURL:           "getPods",
						TestMode:               true,
						FogIP:                  config.Cfg.OverrideFogIP,
						CheatyMinimalFogPing:   float32(closest),
					}

					cfgBytes, err := json.Marshal(enCfg)
					err = ioutil.WriteFile(fmt.Sprintf("%s/edge%d.json", edgePath, counter), cfgBytes, 0644)
					if err != nil {
						//panic(err)
						fmt.Println(err.Error())
					}

					//knownNodes[edgeNode.Name] = "127.0.0.1"
					drawPoint(imgTopo, image.Point{X: int(edgeNode.X), Y: int(edgeNode.Y)}, 5, color.RGBA{0, 150, 0, 255})
				}

				edgeBytes, err := json.Marshal(algorithm.EdgeNodes)
				err = ioutil.WriteFile(fmt.Sprintf("%s/edge.json", edgePath), edgeBytes, 0644)
				if err != nil {
					//panic(err)
					fmt.Println(err.Error())
				}

				f, err := os.Create(fmt.Sprintf("%s/topo.png", edgePath))
				err = png.Encode(bufio.NewWriter(f), imgTopo)
				f.Close()
			}
		}
	}

}

func pickKnownNodes(list map[string]string) map[string]string {
	if len(list) < 5 {
		return list
	}

	nodes := make(map[string]string)
	for i := 0; i < 5; i++ {
		idx := rand.Int31n(int32(len(list)))

		count := int32(0)
		key := ""
		value := ""
		for key, value = range list {

			if count == idx {
				nodes[key] = value
			}
			count++
		}
		//delete(list, key)
	}
	return nodes
}

func drawPoint(img *image.RGBA, p image.Point, size int, c color.Color) {
	for x := 0; x < size; x++ {
		for y := 0; y < size; y++ {
			img.Set(p.X+x, p.Y+y, c)
		}
	}
}

func hslDensityMap(img image.Image) *image.RGBA {
	imgTopo := image.NewRGBA(img.Bounds())

	maxD := float64(0)
	for x := 0; x < img.Bounds().Dx(); x++ {
		for y := 0; y < img.Bounds().Dy(); y++ {
			r, g, b, _ := img.At(x, y).RGBA()
			r /= 256
			g /= 256
			b /= 256
			if b != 255 {
				density := float64(b*255*255 + g*255 + r)
				if density > maxD {
					maxD = density
				}
			}
		}
	}
	gradient := maxD / 255

	for x := 0; x < img.Bounds().Dx(); x++ {
		for y := 0; y < img.Bounds().Dy(); y++ {
			r, g, b, _ := img.At(x, y).RGBA()
			r /= 256
			g /= 256
			b /= 256
			if b != 255 {
				density := float64(b*255*255 + g*255 + r)

				pValue := density / gradient * 360 / 255
				rv, gv, bv := hsvToRgb(180+pValue, 0.5, 1)
				imgTopo.Set(x, y, color.RGBA{rv, gv, bv, 255})
			}
		}
	}
	return imgTopo
}

func hsvToRgb(hue float64, S float64, V float64) (uint8, uint8, uint8) {
	H := hue
	for H < 0 {
		H += 360
	}
	for H >= 360 {
		H -= 360
	}

	R := float64(0)
	G := float64(0)
	B := float64(0)
	if V <= 0 {

	} else if S <= 0 {
		R = V
		G = V
		B = V
	} else {
		hf := H / 60.0
		i := int(math.Floor(hf))
		f := hf - float64(i)
		pv := V * (1 - S)
		qv := V * (1 - S*f)
		tv := V * (1 - S*(1-f))
		switch i {

		// Red is the dominant color

		case 0:
			R = V
			G = tv
			B = pv
			break

		// Green is the dominant color

		case 1:
			R = qv
			G = V
			B = pv
			break
		case 2:
			R = pv
			G = V
			B = tv
			break

		// Blue is the dominant color

		case 3:
			R = pv
			G = qv
			B = V
			break
		case 4:
			R = tv
			G = pv
			B = V
			break

		// Red is the dominant color

		case 5:
			R = V
			G = pv
			B = qv
			break

		// Just in case we overshoot on our math by a little, we put these here. Since its a switch it won't slow us down at all to put these here.

		case 6:
			R = V
			G = tv
			B = pv
			break
		case -1:
			R = V
			G = pv
			B = qv
			break

		// The color is not defined, we should throw an error.

		default:
			//LFATAL("i Value error in Pixel conversion, Value is %d", i);
			//R = G = B = V; // Just pretend its black/white
			R = V
			G = V
			B = V
			break
		}
	}
	r := clamp((int)(R * 255.0))
	g := clamp((int)(G * 255.0))
	b := clamp((int)(B * 255.0))
	return r, g, b
}

func clamp(i int) uint8 {
	if i < 0 {
		return 0
	}
	if i > 255 {
		return 255
	}
	return uint8(i)
}
