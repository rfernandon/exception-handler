package com.rfernandon.exceptionhandler.exceptions;

import static org.springframework.http.HttpStatus.BAD_REQUEST;

public class InvalidParametersException extends ErrorException {

    public InvalidParametersException(String message) {
        super(message, BAD_REQUEST);
    }
}