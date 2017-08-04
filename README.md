# ElevtaorAutoTestFramework
电梯项目自动测试框架

# Demo

启动 ExecuteFiles 中的 RequestGenerator.exe 文件, 它会开启监听本机的8989端口。
启动 ElevatorDemo.exe 文件，它会模拟一个电梯项目向测试框架发送请求。
在成功运行后，RequestGenerator 会产生一个名为 test-2.log 的文件，使用命令行 
`AutoScore.exe Test test-2.log`
即可还原log文件内容。