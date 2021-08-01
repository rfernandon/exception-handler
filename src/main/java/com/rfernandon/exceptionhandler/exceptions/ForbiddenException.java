package com.rfernandon.exceptionhandler.exceptions;

import org.springframework.http.HttpStatus;

public class ForbiddenException extends CallErrorException {

    public ForbiddenException(String message) {
        super(message, HttpStatus.FORBIDDEN);
    }
}
