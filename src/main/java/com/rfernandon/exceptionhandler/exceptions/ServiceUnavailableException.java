package com.rfernandon.exceptionhandler.exceptions;

import static org.springframework.http.HttpStatus.SERVICE_UNAVAILABLE;

public class ServiceUnavailableException extends ErrorException {

    public ServiceUnavailableException(String message) {
        super(message, SERVICE_UNAVAILABLE);
    }
}