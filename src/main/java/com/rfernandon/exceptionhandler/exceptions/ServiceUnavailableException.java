package com.rfernandon.exceptionhandler.exceptions;

import org.springframework.http.HttpStatus;

public class ServiceUnavailableException extends CallErrorException {

    public ServiceUnavailableException(String message) {
        super(message, HttpStatus.SERVICE_UNAVAILABLE);
    }
}
