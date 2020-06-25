package ws

import (
	"encoding/json"
	"fmt"
	"net/http"

	v1 "k8s.io/api/core/v1"
	metav1 "k8s.io/apimachinery/pkg/apis/meta/v1"
	//"github.com/gorilla/mux"
)

//GET /ping
func GetPods(w http.ResponseWriter, r *http.Request) {
	fmt.Println("GetPods")

	pod := v1.Pod{
		ObjectMeta: metav1.ObjectMeta{
			Name: "monitorservice1",
		},
	}
	fmt.Println(pod.Name)
	pods := []v1.Pod{pod}

	jsonBytes, _ := json.Marshal(pods)
	w.Write(jsonBytes)
	//w.WriteHeader(200)

}
