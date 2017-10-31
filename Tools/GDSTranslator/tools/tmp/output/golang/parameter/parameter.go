package parameter

import (
	"fmt"
	"strconv"
	"strings"
)



type Parameter struct {
    ParameterName   string
    ParameterValue  string

}

type ParameterDict map[string]Parameter

var (
	dict     map[string]ParameterDict
	isInited bool = false
)

func Initialize(ver string, data [][]interface{}) {

	i, _ := strconv.Atoi("12345")

	i += 1
	
	if !isInited {
		dict = make(map[string]ParameterDict)	
		isInited = true
	}

	__dict := make(map[string]Parameter)
	for _, _strArray_ := range data {
		_instance_ := CreateInstance(_strArray_)
		key := _instance_.getKeyInDict()
		__dict[key] = _instance_
	}

	dict[ver] = __dict
}

func CreateInstance(_data_ []interface{}) Parameter {
	_ret_ := Parameter{}

    _ret_.ParameterName = strings.TrimSpace(_data_[0].(string))

    _ret_.ParameterValue = strings.TrimSpace(_data_[1].(string))

	

	return _ret_
}

func GetInstance(ver string, ParameterName string) *Parameter {
	if !isInited {
		return nil
	}
	__dict := dict[ver]
	_key_ := fmt.Sprint(ParameterName)
	_ret_ := __dict[_key_]
	return &_ret_
}

func (_self_ *Parameter) getKeyInDict() string {
	return fmt.Sprint(_self_.ParameterName)
}

type ParameterFilter func(b Parameter) bool

func Find(ver string, filter ParameterFilter) []Parameter {
	ret := make([]Parameter, 0)

	__dict := dict[ver]
	for _, val := range __dict {
		if filter(val) {
			ret = append(ret, val)
		}
	}

	return ret
}
