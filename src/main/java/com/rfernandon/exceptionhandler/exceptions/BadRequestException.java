package com.rfernandon.exceptionhandler.exceptions;

import static org.springframework.http.HttpStatus.BAD_REQUEST;

public class BadRequestException extends ErrorException {

    public BadRequestException(String message) {
        super(message, BAD_REQUEST);
    }
}