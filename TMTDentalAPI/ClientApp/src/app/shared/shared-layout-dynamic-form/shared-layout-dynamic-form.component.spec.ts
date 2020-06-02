import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { SharedLayoutDynamicFormComponent } from './shared-layout-dynamic-form.component';

describe('SharedLayoutDynamicFormComponent', () => {
  let component: SharedLayoutDynamicFormComponent;
  let fixture: ComponentFixture<SharedLayoutDynamicFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SharedLayoutDynamicFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SharedLayoutDynamicFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
