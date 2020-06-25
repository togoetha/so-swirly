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

func EdgeRouter() *mux.Router {

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
		Name:        "tryMigrate",
		Method:      "POST",
		Pattern:     "/tryMigrate",
		HandlerFunc: TryMigrate,
		Queries:     []string{},
	},
	Route{
		Name:        "cancelMigrate",
		Method:      "POST",
		Pattern:     "/cancelMigrate",
		HandlerFunc: CancelMigrate,
		Queries:     []string{},
	},
	Route{
		Name:        "migrate",
		Method:      "POST",
		Pattern:     "/migrate",
		HandlerFunc: Migrate,
		Queries:     []string{},
	},
	Route{
		Name:        "getNodeStats",
		Method:      "GET",
		Pattern:     "/getNodeStats",
		HandlerFunc: GetNodeStats,
		Queries:     []string{},
	},
	Route{
		Name:        "getKnownFogNodes",
		Method:      "GET",
		Pattern:     "/getKnownFogNodes",
		HandlerFunc: GetKnownFogNodes,
		Queries:     []string{},
	},
}
