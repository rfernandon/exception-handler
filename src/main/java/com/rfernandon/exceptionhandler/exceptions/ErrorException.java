package com.rfernandon.exceptionhandler.exceptions;

import lombok.Getter;
import org.springframework.http.HttpStatus;

import static org.springframework.http.HttpStatus.INTERNAL_SERVER_ERROR;

@Getter
public class ErrorException extends RuntimeException {

    private HttpStatus httpStatus;

    public ErrorException() {
        this.httpStatus = INTERNAL_SERVER_ERROR;
    }

    public ErrorException(String message) {
        super(message);
        this.httpStatus = INTERNAL_SERVER_ERROR;
    }

    public ErrorException(String message, HttpStatus httpStatus) {
        super(message);
        this.httpStatus = httpStatus;
    }
}