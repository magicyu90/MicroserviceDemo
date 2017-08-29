### 项目所用到的技术

* ASP.NET Core
* Microservice
* RabbitMQ
* RestfulAPI
* Dependency Injection

#### Microservce（微服务）

微服务是一种架构风格，一个大型复杂软件应用由一个或多个微服务组成。系统中的各个微服务可被独立部署，各个微服务之间是松耦合的。每个微服务仅关注于完成一件任务并很好地完成该任务。在所有情况下，每个任务代表着一个小的业务能力。
微服务架构的定义就是一组小型的服务。每一个服务都位于自己的进程中，并且使用诸如HTTP, WebSockets，或者 AMQP 之类的协议进行通讯。它很小，并且专注于做好一件事。

#### RabbitMQ

![RabbitMQ 体系](https://s26.postimg.org/aozana4qx/95517-20170108180648066-1671600.png)

1. 信息发送端将消息(message)发送到exchange

2. exchange接受消息之后，负责将其路由到具体的队列中

3. Bindings负责连接exchange和队列(queue)

4. 消息到达队列(queue)，然后等待被消息接收端处理

5. 消息接收端处理消息