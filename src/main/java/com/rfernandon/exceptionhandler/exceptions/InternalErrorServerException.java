package com.rfernandon.exceptionhandler.exceptions;

import org.springframework.http.HttpStatus;

public class InternalErrorServerException extends CallErrorException {

    public InternalErrorServerException(String message) {
        super(message, HttpStatus.INTERNAL_SERVER_ERROR);
    }
}
