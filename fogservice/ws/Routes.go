package ws

import (
	"github.com/gorilla/mux"

	"net/http"
)

type Route struct {
	Name        string
	Method      string
	Pattern     string
	HandlerFunc http.HandlerFunc
	Queries     []string
}

type Routes []Route

func FogRouter() *mux.Router {

	router := mux.NewRouter().StrictSlash(true)
	for _, route := range routes {
		router.
			Methods(route.Method).
			Path(route.Pattern).
			Name(route.Name).
			Handler(route.HandlerFunc)
		//Queries(route.Queries)
	}

	return router
}

var routes = Routes{
	Route{
		Name:        "ping",
		Method:      "POST",
		Pattern:     "/ping",
		HandlerFunc: Ping,
		Queries:     []string{},
	},
	Route{
		Name:        "isServiceRunning",
		Method:      "POST",
		Pattern:     "/isServiceRunning",
		HandlerFunc: IsServiceRunning,
		Queries:     []string{},
	},
	Route{
		Name:        "addServiceClient",
		Method:      "POST",
		Pattern:     "/addServiceClient",
		HandlerFunc: AddServiceClient,
		Queries:     []string{},
	},
	Route{
		Name:        "removeServiceClient",
		Method:      "POST",
		Pattern:     "/removeServiceClient",
		HandlerFunc: RemoveServiceClient,
		Queries:     []string{},
	},
	Route{
		Name:        "getKnownFogNodes",
		Method:      "GET",
		Pattern:     "/getKnownFogNodes",
		HandlerFunc: GetKnownFogNodes,
		Queries:     []string{},
	},
	Route{
		Name:        "migrateConfirmed",
		Method:      "POST",
		Pattern:     "/migrateConfirmed",
		HandlerFunc: ClientMigrationConfirmed,
		Queries:     []string{},
	},
	Route{
		Name:        "migrateFailed",
		Method:      "POST",
		Pattern:     "/migrateFailed",
		HandlerFunc: ClientMigrationDenied,
		Queries:     []string{},
	},
	Route{
		Name:        "getDiscoveredNodeStats",
		Method:      "GET",
		Pattern:     "/getDiscoveredNodeStats",
		HandlerFunc: GetDiscoveredNodeStats,
		Queries:     []string{},
	},
}
