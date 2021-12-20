import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { TmtAutonumericComponent } from './tmt-autonumeric.component';

describe('TmtAutonumericComponent', () => {
  let component: TmtAutonumericComponent;
  let fixture: ComponentFixture<TmtAutonumericComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ TmtAutonumericComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(TmtAutonumericComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
