package com.rfernandon.exceptionhandler.exceptions;

import static org.springframework.http.HttpStatus.FORBIDDEN;

public class ForbiddenException extends ErrorException {

    public ForbiddenException(String message) {
        super(message, FORBIDDEN);
    }
}