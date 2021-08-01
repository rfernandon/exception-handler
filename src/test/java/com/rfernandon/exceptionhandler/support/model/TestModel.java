package com.rfernandon.exceptionhandler.support.model;

import lombok.Getter;
import lombok.Setter;

import javax.validation.constraints.NotNull;
import javax.validation.constraints.Size;
import java.io.Serializable;

@Getter
@Setter
public class TestModel implements Serializable {

    private static final long serialVersionUID = -7680791462639148032L;

    public TestModel() {
    }

    public TestModel(String name) {
        this.name = name;
    }

    @NotNull
    @Size(min = 10, max = 60)
    private String name;
    @NotNull
    private Integer size;

}
