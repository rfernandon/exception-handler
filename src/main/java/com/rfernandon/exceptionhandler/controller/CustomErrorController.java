package com.rfernandon.exceptionhandler.controller;

import com.rfernandon.exceptionhandler.utils.ErrorMessageFactory;
import org.springframework.boot.autoconfigure.web.ErrorProperties;
import org.springframework.boot.autoconfigure.web.servlet.error.BasicErrorController;
import org.springframework.boot.web.error.ErrorAttributeOptions;
import org.springframework.boot.web.servlet.error.ErrorAttributes;
import org.springframework.http.HttpStatus;
import org.springframework.http.ResponseEntity;
import org.springframework.util.LinkedMultiValueMap;
import org.springframework.util.MultiValueMap;
import org.springframework.web.bind.annotation.RestController;

import javax.servlet.http.HttpServletRequest;
import java.util.Map;

@RestController
public class CustomErrorController extends BasicErrorController {

    public CustomErrorController(ErrorAttributes errorAttributes) {
        super(errorAttributes, new ErrorProperties());
    }

    @Override
    public ResponseEntity<Map<String, Object>> error(HttpServletRequest request) {

        MultiValueMap<String, String> headers = new LinkedMultiValueMap<>();
        headers.add("Content-Type", "application/json");

        Map<String, Object> mapAttributes = this.getErrorAttributes(
                request, ErrorAttributeOptions.of(
                        new ErrorAttributeOptions.Include[]{ErrorAttributeOptions.Include.MESSAGE}));

        HttpStatus status = this.getStatus(request);
        return new ResponseEntity(ErrorMessageFactory.error(
                String.valueOf(status.value()),
                String.format("%s [%s]", status.getReasonPhrase(), mapAttributes.get("message"))), headers, status);
    }
}
