import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ConfigPrintFormComponent } from './config-print-form.component';

describe('ConfigPrintFormComponent', () => {
  let component: ConfigPrintFormComponent;
  let fixture: ComponentFixture<ConfigPrintFormComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ConfigPrintFormComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ConfigPrintFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
