Welcome to the FLEDGE repository!

Repository structure:

* common/algorithm: the general part of the discovery algorithm used in both fog and edge nodes
* dummyfledge: a dummy implementation of FLEDGE, to avoid containers being actually deployed (used in evaluations to reduce interference with memory/CPU measurements)
* edgeservice: the edge node service; edge specific details of discovery algorithm + service watcher that requests required services from fog nodes
* fogservice: the fog node service; fog specific details of discovery algorithm + service/container deployment and load-dependent edge node migration process
* generator: generates a number of edge/fog node config files based on a node density map png. Locations depend on node density and configured algorithm parameters (e.g. max distance)
* netmapgenerator: .NET program that creates node density maps based on population density csv and statistical sector geojson (Belgium data included)
* runner: test program that takes a number of edge/fog node config files (created by the generator) and starts them on localhost

Article available at https://link.springer.com/article/10.1007/s10922-020-09581-6
For more information, contact togoetha.goethals@ugent.be
