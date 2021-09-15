package com.rfernandon.exceptionhandler.utils;

import com.fasterxml.jackson.core.JsonProcessingException;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;

import java.util.List;

import static com.rfernandon.exceptionhandler.support.utils.TestUtil.getJsonNode;
import static com.rfernandon.exceptionhandler.utils.ErrorMessageFactory.error;
import static com.rfernandon.exceptionhandler.utils.ErrorMessageFactory.errorWithParam;
import static org.junit.jupiter.api.Assertions.*;

@DisplayName("\uD83C\uDF7A Testando a factory ErrorMessage")
public class ErrorMessageFactoryTest {

    @Test
    @DisplayName("Should return error message using param")
    public void shouldReturnErrorMessageUsingParam() throws JsonProcessingException {
        var jsonNodes = getJsonNode(errorWithParam("01", "param1"));
        var jsonNode = jsonNodes.get(0);
        assertAll(
                () -> assertTrue(jsonNodes.isArray()),
                () -> assertEquals(1, jsonNodes.size()),
                () -> assertEquals(2, jsonNode.size()),
                () -> assertEquals("01", jsonNode.get("code").asText()),
                () -> assertEquals("Test message: param1", jsonNode.get("message").asText())
        );
    }

    @Test
    @DisplayName("Should return error message using code error")
    public void shouldReturnErrorMessageUsingCodeError() throws JsonProcessingException {
        var jsonNodes = getJsonNode(error("02"));
        var jsonNode = jsonNodes.get(0);
        assertAll(
                () -> assertTrue(jsonNodes.isArray()),
                () -> assertEquals(1, jsonNodes.size()),
                () -> assertEquals(2, jsonNode.size()),
                () -> assertEquals("02", jsonNode.get("code").asText()),
                () -> assertEquals("Test message", jsonNode.get("message").asText())
        );
    }

    @Test
    @DisplayName("should return error message")
    public void shouldReturnErrorMessage() throws JsonProcessingException {
        var jsonNodes = getJsonNode(error("00", "message error"));
        var jsonNode = jsonNodes.get(0);
        assertAll(
                () -> assertTrue(jsonNodes.isArray()),
                () -> assertEquals(1, jsonNodes.size()),
                () -> assertEquals(2, jsonNode.size()),
                () -> assertEquals("00", jsonNode.get("code").asText()),
                () -> assertEquals("message error", jsonNode.get("message").asText())
        );
    }

    @Test
    @DisplayName("should return error message with too many messages")
    public void shouldReturnErrorMessageWithManyMessages() throws JsonProcessingException {
        var messageErrorFactory = new ErrorMessageFactory();
        messageErrorFactory.add("02");
        messageErrorFactory.add("02", "Test message 02");

        var jsonNodes = getJsonNode(messageErrorFactory.build());

        assertAll(
                () -> assertTrue(jsonNodes.isArray()),
                () -> assertEquals(2, jsonNodes.size())
        );

        jsonNodes.elements().forEachRemaining(n ->
                assertAll(
                        () -> assertEquals(2, n.size()),
                        () -> assertEquals("02", n.get("code").asText()),
                        () -> assertTrue(List.of("Test message","Test message 02").contains(n.get("message").textValue()))
                )
        );
    }
}