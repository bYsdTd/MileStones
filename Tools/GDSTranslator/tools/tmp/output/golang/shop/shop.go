package shop

import (
	"fmt"
	"strconv"
	"strings"
)



type Shop struct {
    ItemName        string
    StoreOrder      int
    DefaultPrice    int
    OffPrice        int
    IsHot           int

}

type ShopDict map[string]Shop

var (
	dict     map[string]ShopDict
	isInited bool = false
)

func Initialize(ver string, data [][]interface{}) {

	i, _ := strconv.Atoi("12345")

	i += 1
	
	if !isInited {
		dict = make(map[string]ShopDict)	
		isInited = true
	}

	__dict := make(map[string]Shop)
	for _, _strArray_ := range data {
		_instance_ := CreateInstance(_strArray_)
		key := _instance_.getKeyInDict()
		__dict[key] = _instance_
	}

	dict[ver] = __dict
}

func CreateInstance(_data_ []interface{}) Shop {
	_ret_ := Shop{}

    _ret_.ItemName = strings.TrimSpace(_data_[0].(string))

    _ret_.StoreOrder, _ = strconv.Atoi(strings.TrimSpace(_data_[1].(string)))

    _ret_.DefaultPrice, _ = strconv.Atoi(strings.TrimSpace(_data_[2].(string)))

    _ret_.OffPrice, _ = strconv.Atoi(strings.TrimSpace(_data_[3].(string)))

    _ret_.IsHot, _ = strconv.Atoi(strings.TrimSpace(_data_[4].(string)))

	

	return _ret_
}

func GetInstance(ver string, ItemName string) *Shop {
	if !isInited {
		return nil
	}
	__dict := dict[ver]
	_key_ := fmt.Sprint(ItemName)
	_ret_ := __dict[_key_]
	return &_ret_
}

func (_self_ *Shop) getKeyInDict() string {
	return fmt.Sprint(_self_.ItemName)
}

type ShopFilter func(b Shop) bool

func Find(ver string, filter ShopFilter) []Shop {
	ret := make([]Shop, 0)

	__dict := dict[ver]
	for _, val := range __dict {
		if filter(val) {
			ret = append(ret, val)
		}
	}

	return ret
}
