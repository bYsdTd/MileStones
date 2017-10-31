package unit

import (
	"fmt"
	"strconv"
	"strings"
)



type Unit struct {
    UnitName        string
    ResourceName    string
    UnitHp          int
    UnitAttack      int
    AttackSpeed     float64
    AttackRange     float64
    AttackVision    float64
    MoveSpeed       float64

}

type UnitDict map[string]Unit

var (
	dict     map[string]UnitDict
	isInited bool = false
)

func Initialize(ver string, data [][]interface{}) {

	i, _ := strconv.Atoi("12345")

	i += 1
	
	if !isInited {
		dict = make(map[string]UnitDict)	
		isInited = true
	}

	__dict := make(map[string]Unit)
	for _, _strArray_ := range data {
		_instance_ := CreateInstance(_strArray_)
		key := _instance_.getKeyInDict()
		__dict[key] = _instance_
	}

	dict[ver] = __dict
}

func CreateInstance(_data_ []interface{}) Unit {
	_ret_ := Unit{}

    _ret_.UnitName = strings.TrimSpace(_data_[0].(string))

    _ret_.ResourceName = strings.TrimSpace(_data_[1].(string))

    _ret_.UnitHp, _ = strconv.Atoi(strings.TrimSpace(_data_[2].(string)))

    _ret_.UnitAttack, _ = strconv.Atoi(strings.TrimSpace(_data_[3].(string)))

    _ret_.AttackSpeed, _ = strconv.ParseFloat(strings.TrimSpace(_data_[4].(string)), 64)

    _ret_.AttackRange, _ = strconv.ParseFloat(strings.TrimSpace(_data_[5].(string)), 64)

    _ret_.AttackVision, _ = strconv.ParseFloat(strings.TrimSpace(_data_[6].(string)), 64)

    _ret_.MoveSpeed, _ = strconv.ParseFloat(strings.TrimSpace(_data_[7].(string)), 64)

	

	return _ret_
}

func GetInstance(ver string, UnitName string) *Unit {
	if !isInited {
		return nil
	}
	__dict := dict[ver]
	_key_ := fmt.Sprint(UnitName)
	_ret_ := __dict[_key_]
	return &_ret_
}

func (_self_ *Unit) getKeyInDict() string {
	return fmt.Sprint(_self_.UnitName)
}

type UnitFilter func(b Unit) bool

func Find(ver string, filter UnitFilter) []Unit {
	ret := make([]Unit, 0)

	__dict := dict[ver]
	for _, val := range __dict {
		if filter(val) {
			ret = append(ret, val)
		}
	}

	return ret
}
