package com.rfernandon.exceptionhandler.exceptions;

import org.springframework.http.HttpStatus;

public class InvalidParametersException extends CallErrorException {

    public InvalidParametersException(String message) {
        super(message, HttpStatus.BAD_REQUEST);
    }
}
