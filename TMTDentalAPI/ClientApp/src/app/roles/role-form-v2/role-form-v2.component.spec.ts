import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RoleFormV2Component } from './role-form-v2.component';

describe('RoleFormV2Component', () => {
  let component: RoleFormV2Component;
  let fixture: ComponentFixture<RoleFormV2Component>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RoleFormV2Component ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RoleFormV2Component);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
