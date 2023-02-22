import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { ZaloOaConfigEstablishComponent } from './zalo-oa-config-establish.component';

describe('ZaloOaConfigEstablishComponent', () => {
  let component: ZaloOaConfigEstablishComponent;
  let fixture: ComponentFixture<ZaloOaConfigEstablishComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ ZaloOaConfigEstablishComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(ZaloOaConfigEstablishComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
