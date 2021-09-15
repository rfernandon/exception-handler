package com.rfernandon.exceptionhandler.exceptions;

import static org.springframework.http.HttpStatus.UNAUTHORIZED;

public class UnauthorizedException extends ErrorException {

    public UnauthorizedException(String message) {
        super(message, UNAUTHORIZED);
    }
}