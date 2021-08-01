package com.rfernandon.exceptionhandler.exceptions;

import org.springframework.http.HttpStatus;

public class NotFoundException extends CallErrorException {

    public NotFoundException(String message) {
        super(message, HttpStatus.NOT_FOUND);
    }
}
