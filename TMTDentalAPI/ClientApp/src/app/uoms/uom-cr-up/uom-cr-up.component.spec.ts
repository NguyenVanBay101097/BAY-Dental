import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { UomCrUpComponent } from './uom-cr-up.component';

describe('UomCrUpComponent', () => {
  let component: UomCrUpComponent;
  let fixture: ComponentFixture<UomCrUpComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UomCrUpComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UomCrUpComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
