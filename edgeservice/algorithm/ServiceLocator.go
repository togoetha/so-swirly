package algorithm

import (
	"fmt"
	"os/exec"
)

var Locator ServiceLocator

type ServiceLocator interface {
	Init() ServiceLocator
	UpdateServiceLocation(serviceName string, newIP string) bool
}

type HostsServiceLocator struct {
}

func (sl *HostsServiceLocator) Init() ServiceLocator {
	return sl
}

func (sl *HostsServiceLocator) UpdateServiceLocation(serviceName string, newIP string) bool {
	removeHostsLine(serviceName)
	addHostsLine(serviceName, newIP)
	return true
}

func removeHostsLine(host string) {
	cmd := fmt.Sprintf("cat /etc/hosts | grep -w -v \"%s\" > /etc/hosts", host)
	ExecCmdBash(cmd)
}

func addHostsLine(host string, ip string) {
	line := fmt.Sprintf("%s	%s", host, ip)
	cmd := fmt.Sprintf("echo \"%s\" >> /etc/hosts", line)
	ExecCmdBash(cmd)
}

func ExecCmdBash(dfCmd string) (string, error) {
	fmt.Printf("Executing %s\n", dfCmd)
	cmd := exec.Command("sh", "-c", dfCmd)
	stdout, err := cmd.Output()

	if err != nil {
		println(err.Error())
		return "", err
	}
	//fmt.Println(string(stdout))
	return string(stdout), nil
}
