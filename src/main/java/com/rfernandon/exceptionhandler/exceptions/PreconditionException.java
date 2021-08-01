package com.rfernandon.exceptionhandler.exceptions;

import org.springframework.http.HttpStatus;

public class PreconditionException extends CallErrorException {

    public PreconditionException(String message) {
        super(message, HttpStatus.PRECONDITION_FAILED);
    }
}
