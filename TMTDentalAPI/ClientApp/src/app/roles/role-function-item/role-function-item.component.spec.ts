import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { RoleFunctionItemComponent } from './role-function-item.component';

describe('RoleFunctionItemComponent', () => {
  let component: RoleFunctionItemComponent;
  let fixture: ComponentFixture<RoleFunctionItemComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ RoleFunctionItemComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(RoleFunctionItemComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
