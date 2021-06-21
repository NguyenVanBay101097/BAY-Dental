import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ToaThuocLineUseatPopoverComponent } from './toa-thuoc-line-useat-popover.component';

describe('ToaThuocLineUseatPopoverComponent', () => {
  let component: ToaThuocLineUseatPopoverComponent;
  let fixture: ComponentFixture<ToaThuocLineUseatPopoverComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ToaThuocLineUseatPopoverComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ToaThuocLineUseatPopoverComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
