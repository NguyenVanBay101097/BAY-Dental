import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ConfigPrintManagementComponent } from './config-print-management.component';

describe('ConfigPrintManagementComponent', () => {
  let component: ConfigPrintManagementComponent;
  let fixture: ComponentFixture<ConfigPrintManagementComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ConfigPrintManagementComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ConfigPrintManagementComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
