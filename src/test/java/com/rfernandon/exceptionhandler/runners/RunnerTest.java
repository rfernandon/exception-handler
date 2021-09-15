package com.rfernandon.exceptionhandler.runners;

import io.cucumber.junit.Cucumber;
import io.cucumber.junit.CucumberOptions;
import org.junit.jupiter.api.BeforeAll;
import org.junit.runner.RunWith;

@RunWith(Cucumber.class)
@CucumberOptions(
        tags = { "@CustomErrorSpring or @CustomErrorApp" },     // para negar, usar: "~@ignore"
        features = "src/test/resources/features",
        glue = "com.rfernandon.exceptionhandler.steps",         // pacote onde o cucumber irá enconttrar os steps
        plugin = {
                "pretty",
                "html:target/cucumber-html",
                "json:target/cucumber-json"
        },
        monochrome = false,                                      // Remove as cores da saída dos testes
        snippets = CucumberOptions.SnippetType.CAMELCASE,        // Gera os nomes dos metodos no formato camelCase
        strict = true)
public class RunnerTest {

        @BeforeAll
        public void beforeAll() {
        }
}