# exception-handler

![version](https://img.shields.io/badge/version-1.0.0-blue)
[![Java Version](https://img.shields.io/static/v1?label=java&message=11&color=green)](https://openjdk.java.net/projects/jdk/11)
[![Spring Version](https://img.shields.io/static/v1?label=spring%20boot&message=2.3.10.RELEASE&color=green)](https://spring.io)
[![Build Status](https://travis-ci.com/rfernandon/exception-handler.svg?branch=master)](https://travis-ci.com/rfernandon/exception-handler) 
[![Sonarcloud Quality Gate](https://sonarcloud.io/api/project_badges/measure?project=com.rfernandon:exception-handler&metric=alert_status)](https://sonarcloud.io/dashboard?id=com.rfernandon:exception-handler) 
[![Sonarcloud Coverage](https://sonarcloud.io/api/project_badges/measure?project=com.rfernandon:exception-handler&metric=coverage)](https://sonarcloud.io/component_measures?id=com.rfernandon:exception-handler)

Esse componente foi desenvolvido para ser usado em aplicações `Spring Boot`, ele implementa um handler que captura as exceptions lançadas pelas aplicações e transforma as mensagens em um formato mais amigável.

### Como usar

Quando a aplicação lançar uma exception, a aplicação cliente irá receber uma mensagem de erro no seguinte formato JSON:
```json
[
  {
    "code": "01",
    "message": "Mensagem de erro gerada pela aplicação"
  }
]
```

Para retornar a mensagen, basta lançar um exception utilizando a classe **ErrorMessageFactory** para definir a mensagem de erro:
```java
throw new BadRequestException(ErrorMessageFactory.error("01"));
```

O código "01" pode ser configurado em um arquivo **messages_en.properties**:
```properties
01=Mensagem de erro gerada pela aplicação
02=O erro {0} foi gerado pela aplicação
```

Podemos também passar parâmetros nas mensagens de erro dessa forma:
```java
throw new BadRequestException(ErrorMessageFactory.errorWithParam("02", "XPTO"));
```

O componente também oferece um conjunto de exceptions que podem ser usadas nas aplicações:

- BadRequestException
- ErrorException
- ForbiddenException
- InvalidParametersException
- NotFoundException
- NotImplementedException
- PreconditionException
- ServiceUnavailableException
- UnauthorizedException
- UnprocessableEntityException

Para adicionar a lib em seu projeto, inclua o seguinte repositório em seu pom.xml:

```xml
<repositories>
    <repository>
        <id>rfernandon-repo</id>
        <url>https://raw.github.com/rfernandon/repository-maven/main/</url>
        <snapshots>
            <enabled>true</enabled>
            <updatePolicy>always</updatePolicy>
        </snapshots>
    </repository>
</repositories>
```

Em seguida adicione a dependência:

```xml
<dependency>
    <groupId>com.rfernandon</groupId>
    <artifactId>exception-handler</artifactId>
    <version>1.0.0</version>
</dependency>
```