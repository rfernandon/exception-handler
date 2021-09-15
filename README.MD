[![Build Status](https://travis-ci.com/rfernandon/exception-handler.svg?branch=master)](https://travis-ci.com/rfernandon/exception-handler) 
[![Sonarcloud Quality Gate](https://sonarcloud.io/api/project_badges/measure?project=com.rfernandon:exception-handler&metric=alert_status)](https://sonarcloud.io/dashboard?id=com.rfernandon:exception-handler) 
[![Sonarcloud Coverage](https://sonarcloud.io/api/project_badges/measure?project=com.rfernandon:exception-handler&metric=coverage)](https://sonarcloud.io/component_measures?id=com.rfernandon:exception-handler)

# exception-handler
Componente desenvolvido para disponibilizar um handler que captura as exceptions lançadas pelas aplicações.

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
