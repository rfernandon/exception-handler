package com.rfernandon.exceptionhandler.support.controller;

import com.rfernandon.exceptionhandler.factory.ErrorMessageFactory;
import com.rfernandon.exceptionhandler.support.model.TestModel;
import lombok.extern.slf4j.Slf4j;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import java.lang.reflect.Constructor;
import javax.validation.Valid;

@Slf4j
@RestController
public class TestController {

    @GetMapping("/internalerrorserver")
    public ResponseEntity<Object> internalerrorserver() {
        return ResponseEntity.ok("".substring(10));
    }

    @GetMapping("/simulation/{code}")
    public ResponseEntity<Object> simulation(@PathVariable("code") String code, @RequestParam("param") String param)
            throws Throwable {

        Class<?> cl = Class.forName(String.format("com.rfernandon.exceptionhandler.exceptions.%s", param));
        Constructor<?> constructor = cl.getConstructor(String.class);
        Throwable newInstance = (Throwable) constructor.newInstance(ErrorMessageFactory.errorWithParam(code, param));
        throw newInstance;
    }

    @PostMapping(value = "/simulation", consumes={"application/json"}, produces={"application/json"})
    public ResponseEntity<Object> simulation(@Valid @RequestBody TestModel testModel) throws Throwable {
        return ResponseEntity.ok().body("{ \"value\" : \"ok\" }");
    }
}