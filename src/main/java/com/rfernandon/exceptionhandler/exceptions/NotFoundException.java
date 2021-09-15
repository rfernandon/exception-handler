package com.rfernandon.exceptionhandler.exceptions;

import static org.springframework.http.HttpStatus.NOT_FOUND;

public class NotFoundException extends ErrorException {

    public NotFoundException(String message) {
        super(message, NOT_FOUND);
    }
}