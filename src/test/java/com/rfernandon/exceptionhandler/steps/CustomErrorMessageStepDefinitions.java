package com.rfernandon.exceptionhandler.steps;

import com.fasterxml.jackson.core.JsonProcessingException;
import com.rfernandon.exceptionhandler.support.model.TestModel;
import io.cucumber.datatable.DataTable;
import io.cucumber.java.After;
import io.cucumber.java.Before;
import io.cucumber.java.en.Given;
import io.cucumber.java.en.Then;
import io.cucumber.java.en.When;
import io.cucumber.spring.CucumberContextConfiguration;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.test.context.SpringBootTest;
import org.springframework.boot.test.web.client.TestRestTemplate;
import org.springframework.http.HttpEntity;
import org.springframework.http.ResponseEntity;

import java.beans.Statement;
import java.lang.reflect.Constructor;

import static com.rfernandon.exceptionhandler.support.utils.TestUtil.getJsonNode;
import static org.junit.jupiter.api.Assertions.*;

@CucumberContextConfiguration
@SpringBootTest(webEnvironment = SpringBootTest.WebEnvironment.RANDOM_PORT)
public class CustomErrorMessageStepDefinitions {

    @Autowired
    private TestRestTemplate restTemplate;

    private String url;
    private Object object;
    private ResponseEntity<String> responseEntity;

    @Before
    public void start() {
    }

    @After
    public void finish() {
    }

    @Given("the endpoint {string}")
    public void theEndpoint(String uri) {
        url = uri;
    }

    @Given("the endpoint {string} with path {string} and param {string}")
    public void theEndpointWithParamBadRequestException(String endpoint, String code, String param) {
        url = endpoint + "/" + code + "?param=" + param;
    }

    @Given("the class {string} in package {string}")
    public void theObject(String _class, String _package) throws Exception {
        Class<?> clazz = Class.forName(String.format("%s.%s", _package, _class));
        Constructor<?> constructor = clazz.getConstructor();
        object = constructor.newInstance();
    }

    @Given("with value {string} in field {string}")
    public void withValueInField(String value, String prop) throws Exception {
        Statement stmt = new Statement(object, prop, new Object[] { value });
        stmt.execute();
    }

    @When("the customer calls the endpoint using method GET")
    public void theCustomerCallsTheEndpointUsingMethodGET() {
        responseEntity = restTemplate.getForEntity(url, String.class);
    }

    @When("the customer calls the endpoint using method POST")
    public void theCustomerCallsTheEndpointUsingMethodPOST() {

        HttpEntity<TestModel> request = new HttpEntity<>(new TestModel("123"));
        responseEntity = restTemplate.postForEntity(url, request, String.class);
    }

    @Then("the client receives status http of {int}")
    public void theClientReceivesStatusHttpOf(Integer status) {
        assertEquals(status, responseEntity.getStatusCode().value());
    }

    @Then("the client receives server message {string} and code {string}")
    public void theClientReceivesServerMessageAndCode(String message, String code) throws JsonProcessingException {
        var body = responseEntity.getBody();
        var jsonNodes = getJsonNode(body);
        var jsonNode = jsonNodes.get(0);

        assertAll(
                () -> assertTrue(jsonNodes.isArray()),
                () -> assertEquals(1, jsonNodes.size()),
                () -> assertEquals(2, jsonNode.size()),
                () -> assertEquals(code, jsonNode.get("code").asText()),
                () -> assertEquals(message, jsonNode.get("message").asText())
        );
    }

    @Then("the client receives server code {string} with the validation messages:")
    public void theClientReceivesServerCodeWithTheValidationMessages(String code, DataTable dataTable) throws JsonProcessingException {
        var body = responseEntity.getBody();
        var jsonNodes = getJsonNode(body);

        assertAll(
                () -> assertTrue(jsonNodes.isArray()),
                () -> assertEquals(2, jsonNodes.size())
        );

        jsonNodes.elements().forEachRemaining(n ->
            assertAll(
                    () -> assertEquals(2, n.size()),
                    () -> assertEquals(code, n.get("code").asText()),
                    () -> assertTrue(dataTable.asList().contains(n.get("message").textValue()))
            )
        );
    }
}
