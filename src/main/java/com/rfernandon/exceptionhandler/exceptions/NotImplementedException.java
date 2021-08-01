package com.rfernandon.exceptionhandler.exceptions;

import org.springframework.http.HttpStatus;

public class NotImplementedException extends CallErrorException {

    public NotImplementedException(String message) {
        super(message, HttpStatus.NOT_IMPLEMENTED);
    }
}
