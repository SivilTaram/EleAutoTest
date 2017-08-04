# EleAutoTest

电梯项目自动测试框架

# Start

1. 启动 ExecuteFiles 中的 RequestGenerator.exe 文件, 它会监听本机的8989端口，正确运行的结果如下

```json
RequestGenerator >> Start Listening in 127.0.0.1:8989
```
2. 启动 ElevatorDemo.exe 文件，它会模拟一个电梯项目向测试框架发送请求，正确运行时Client端结果如下
```json
Client >> Connecting to autotest framework
Client >> Connected to autotest framework
Client >> Send Data :    
{
    "User":"Student",
    "Elevators":
    [
        {
            "ID": 1,
            "Capability": 1500,
            "FloorMax": 25,
            "FloorHeight": 10,
            "InitHeight": 20
        }
    ],
    "TaskID":"2",
    "Operation":"CONFIG"
}
Client >>Server Response :       Config Success
Client >> Send Data :    
{
    "Tick":0,
    "FinishRequests":[],
    "Operation":"GETREQS"
}
Client >>Server Response :       
{
    "Passengers":
    [
        "Sen_1,15,12,63"
    ],
    "NextTick":-1
}
Client >> Send Data :    
{
    "Tick":-1,
    "FinishRequests":[
        {
            "PassengerName":"Sen_1",
            "FinishTime":20,
            "ElevatorID":1
        }
    ],
    "Operation":"GETREQS"
}
Client >>Server Response :       
{
    "Correct":true,
    "Cost":20.0,
    "Standard":360.0
}
```
3. 在请求评判结束后，RequestGenerator 会产生一个名为 **test-2.log** 的文件，它是加密后的日志，打开查看可以看到
```
B1LKEBobxLdOnWdS5X56jk9K7OFq4vatehZgIKioQjDTen2y5CD3oYs8ekYZTMHBJYa/JGJmv32v/SZGdwik6YsukyglB2m8eSOq9TwYFak1tM2ACSlicOPkZBEHidMCJwrOaj0qzR6k3fdQwuGR24SF5bADOL4Pk6CuH4iFrbTI+bej3nleBEOCu0jusLtIJKXgJlP888XiNaVt97SNtePD1IWw9qyRXXvF7Lk8j+MGRQqjPwm5Qc3TfWqPJDsChb+I5G9JS8iLRJVw4m6vHEbTDGCbJ8AFBtsf0ilN01mH4aX6/HREDyWmG4OPXeylurRiHXQ4TxMER5KVgghN+L+811csNEEUJ2zlflnBUksAXLOZ1zYfUPHypMCdq0c0MPem3bgMKa6KeGtnUGneHL44tYvthmo3YH8kdNaAbVKkvzyAMD7tr4qQF/IRXFHnpRc6Cjca8c+s4LMQsLa+jnIxD9KZcONuU8VtYZ2ZdzCzFQjel2C05sDtVxK3IeouMnE6u//+XgQcJUfpvu5CTGIaQBT7n15tjdbd93O76ChrGohqI+mGZ381AWGIbJLm/AGYLiGTi6Xxz3VOi4hZ6zYGOo5IRLQqBUIjHFruZTwi3YwJzphNW1sLBChMCixNIs47wjQ3wxRym84xrFew5nw/GHsBru2oFp5Lbpqegt4=
```

此时使用命令行 

```shell
AutoScore.exe Test test-2.log`
```
可还原log文件内容，还原后的内容实际上是对测试程序的评分。